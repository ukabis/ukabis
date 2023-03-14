using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JP.DataHub.Com.Net.Http.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    public abstract class TestDataBase
    {
        protected const string WILDCARD = "{{*}}";
        protected const string DefaultPartitionKeyRoot = "API~IntegratedTest~";
        protected const string DefaultResourceUrlRoot = "/API/IntegratedTest/";

        public string PartitionKeyRoot { get; private set; } = null;
        public string ResourceUrl { get; private set; } = null;

        private Repository _repository = Repository.Default;
        private string _resourceUrl;
        private bool _isVendor = false;
        private bool _isPerson = false;
        private string _vendorId;
        private string _systemId;
        private string _openId;

        protected virtual string BaseRepositoryKeyPrefix { get; set; } = null;


        public TestDataBase(Repository repository = Repository.Default, string resourceUrl = null, bool isVendor = false, bool isPerson = false, IntegratedTestClient client = null)
        {
            _repository = repository;
            _resourceUrl = resourceUrl;
            _isVendor = isVendor;
            _isPerson = isPerson;
            if (client != null)
            {
                _vendorId = client.VendorSystemInfo.VendorId;
                _systemId = client.VendorSystemInfo.SystemId;
                _openId = client.GetOpenId();
            }

            if (string.IsNullOrEmpty(resourceUrl))
            {
                return;
            }

            // MongoCDSは依存なしは有り得ないためデフォルトでベンダー依存と看做す。
            if (_repository == Repository.MongoDbCds && _isVendor == false && _isPerson == false)
            {
                _isVendor = true;
            }

            // パーティションキー
            var partitionKeyRoot = DefaultPartitionKeyRoot;
            switch (_repository)
            {
                case Repository.CosmosDb: break;
                case Repository.MongoDb: partitionKeyRoot = "API~IntegratedTest~Mongo~"; break;
                case Repository.MongoDbCds: partitionKeyRoot = "API~IntegratedTest~MongoCDS~"; break;
                case Repository.SqlServer: partitionKeyRoot = "API~IntegratedTest~SqlServer~"; break;
                default: break;
            }
            PartitionKeyRoot = partitionKeyRoot.Substring(0, partitionKeyRoot.Length - 1);

            var partitionKeyPrefix = _resourceUrl.Replace("/", "~");
            if (partitionKeyPrefix.StartsWith("~"))
            {
                partitionKeyPrefix = partitionKeyPrefix[1..];
            }
            var newPartitionKeyPrefix = partitionKeyPrefix.Replace(DefaultPartitionKeyRoot, partitionKeyRoot);

            // リソースURL
            switch (_repository)
            {
                case Repository.CosmosDb: ResourceUrl = _resourceUrl; break;
                case Repository.MongoDb: ResourceUrl = _resourceUrl.Replace(DefaultResourceUrlRoot, "/API/IntegratedTest/Mongo/"); break;
                case Repository.MongoDbCds: ResourceUrl = _resourceUrl.Replace(DefaultResourceUrlRoot, "/API/IntegratedTest/MongoCDS/"); break;
                case Repository.SqlServer: ResourceUrl = _resourceUrl.Replace(DefaultResourceUrlRoot, "/API/IntegratedTest/SqlServer/"); break;
                default: break;
            }

            // テストデータの各プロパティを条件に応じて更新する。
            var fields = this.GetType().GetFields();
            foreach (var field in fields.Where(x => x.Name.StartsWith("Data", StringComparison.OrdinalIgnoreCase)))
            {
                var isAttachFileMeta = field.FieldType == typeof(GetAttachFileResponseModel) || field.FieldType == typeof(List<GetAttachFileResponseModel>);
                var data = field.GetValue(this);
                if (field.FieldType.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    UpdatePropertyEnumerable(data, (BaseRepositoryKeyPrefix ?? partitionKeyPrefix), newPartitionKeyPrefix, isAttachFileMeta);
                }
                else if (Type.GetTypeCode(field.FieldType) == TypeCode.Object)
                {
                    var dataProperties = data.GetType().GetProperties();
                    foreach (var dataProperty in dataProperties.Where(x => x.SetMethod != null))
                    {
                        UpdateProperty(data, dataProperty, (BaseRepositoryKeyPrefix ?? partitionKeyPrefix), newPartitionKeyPrefix, isAttachFileMeta);
                    }
                }
            }
        }

        private void UpdatePropertyEnumerable(object data, string oldPrefix, string newPrefix, bool isAttachFileMeta)
        {
            if (data == null)
            {
                return;
            }

            foreach (var obj in (IEnumerable)data)
            {
                if (obj == null)
                {
                    continue;
                }

                var type = obj.GetType();
                if (type.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    UpdatePropertyEnumerable(obj, oldPrefix, newPrefix, isAttachFileMeta);
                }
                else if (Type.GetTypeCode(type) == TypeCode.Object)
                {
                    var objProperties = obj.GetType().GetProperties();
                    foreach (var objProperty in objProperties.Where(x => x.SetMethod != null))
                    {
                        UpdateProperty(obj, objProperty, oldPrefix, newPrefix, isAttachFileMeta);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private void UpdateProperty(object data, PropertyInfo property, string oldPrefix, string newPrefix, bool isAttachFileMeta)
        {
            // 更新処理
            if (property.PropertyType == typeof(string))
            {
                var value = (string)property.GetValue(data);
                if (value == null)
                {
                    return;
                }

                // URLセグメント更新
                if (property.Name == "id" || property.Name == "_partitionkey" || property.Name == "_Type")
                {
                    value = value.Replace(oldPrefix, newPrefix);
                }

                // 依存セグメント更新
                if (property.Name == "id" || property.Name == "_partitionkey")
                {
                    var segments = value.Replace($"{newPrefix}~", "").Split(new char[] { '~' });
                    var version = segments[0];
                    var keys = segments.Length > 1 ? $"~{string.Join("~", segments.Skip(1))}" : "";
                    var vendorId = _vendorId ?? WILDCARD;
                    var systemId = _systemId ?? WILDCARD;
                    var openId = _openId ?? WILDCARD;
                    if (!isAttachFileMeta)
                    {
                        if (_isPerson && _isVendor)
                        {
                            if (property.Name == "id")
                            {
                                value = $"{newPrefix}~{openId}~{vendorId}~{systemId}~{version}{keys}";
                            }
                            else
                            {
                                value = $"{newPrefix}~{vendorId}~{systemId}~{openId}~{version}{keys}";
                            }
                        }
                        else if (_isPerson)
                        {
                            value = $"{newPrefix}~{openId}~{version}{keys}";
                        }
                        else if (_isVendor)
                        {
                            value = $"{newPrefix}~{vendorId}~{systemId}~{version}{keys}";
                        }
                    }
                }

                // リポジトリ別編集
                if (_repository == Repository.SqlServer)
                {
                    if (property.Name == "_partitionkey")
                    {
                        value = null;
                    }
                    if (property.Name == "_Type" && !isAttachFileMeta)
                    {
                        value = null;
                    }
                }

                // 依存別編集
                if (!_isVendor)
                {
                    if (property.Name == "_Vendor_Id" || property.Name == "_System_Id")
                    {
                        value = null;
                    }
                }

                property.SetValue(data, value);
            }
            else if (property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
            {
                var obj = property.GetValue(data);
                UpdatePropertyEnumerable(obj, oldPrefix, newPrefix, isAttachFileMeta);
            }
            else if (Type.GetTypeCode(property.PropertyType) == TypeCode.Object)
            {
                var obj = property.GetValue(data);
                if (obj != null)
                {
                    var childProperties = obj.GetType().GetProperties();
                    foreach (var childProperty in childProperties.Where(x => x.SetMethod != null))
                    {
                        UpdateProperty(obj, childProperty, oldPrefix, newPrefix, isAttachFileMeta);
                    }
                }
            }
        }
    }
}
