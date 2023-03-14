using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class RegisterRawDataAction : AbstractDynamicApiAction, IEntity
    {
        private static readonly IMapper mapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<IPerRequestDataContainer, IPerRequestDataContainer>();
        }).CreateMapper();

        [DataContract]
        private class RegistReturnId
        {
            [DataMember]
            public string id { get; set; }

            public bool IsError { get; set; } = false;
            public string Error { get; set; }
        }

        public override HttpResponseMessage ExecuteAction()
        {
            if ((this.MethodType.IsPost != true) && (this.MethodType.IsPut != true))
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }

            if (string.IsNullOrEmpty(this.RepositoryKey.Value))
            {
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, null));
            }

            // 運用管理ベンダー以外の呼び出しはForbbiden(暫定)
            // 将来的にはAPIの所有ベンダーであれば許可したい
            if (!PerRequestDataContainer.IsOperatingVendorUser)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E10441, RelativeUri?.Value);
            }

            // jsonデータ
            JToken json;
            try
            {
                json = ToJson(Contents.ReadToString());
            }
            catch (Exception ex)
            {
                var log = new JPDataHubLogger(this.GetType());
                log.Error(ex);
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10403, this.RelativeUri?.Value);
                msg.Detail = ex.Message.Replace("\r", "").Replace("\n", "");
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, msg.ToJson().ToString()));
            }


            //配列以外はNG
            if (json == null || json.Type != JTokenType.Array)
            {
                var msg = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10405, this.RelativeUri?.Value);
                this.MediaType = new MediaType(MEDIATYPE_ProblemJson);
                return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.BadRequest, msg.ToJson().ToString()));
            }

            // コンテナ分離の場合は登録者ではなくデータの属性(ベンダーID/システムID、OpenID)で格納先のコンテナを決定する必要がある。
            // PerRequestDataContainerの値でコンテナが決定されるため、データごとにPerRequestDataContainerの値を書き換えたうえで登録を行う。
            List<RegistReturnId> regids = new List<RegistReturnId>();
            var originalVendorId = PerRequestDataContainer.VendorId;
            var originalSystemId = PerRequestDataContainer.SystemId;
            var originalOpenId = PerRequestDataContainer.OpenId;
            try
            {
                for (int i = 0; i < json.Count(); i++)
                {
                    var admins = CalcAdminFields(this.DynamicApiDataStoreRepository[0], json[i]);
                    MergeAdminFields(json[i], json[i], admins);
                    if (IsContainerDynamicSeparation?.Value == true)
                    {
                        if (IsVendor?.Value == true)
                        {
                            var vendorId = json[i][VENDORID].Value<string>();
                            var systemId = json[i][SYSTEMID].Value<string>();
                            PerRequestDataContainer.VendorId = vendorId;
                            PerRequestDataContainer.SystemId = systemId;
                        }
                        if (IsPerson?.Value == true)
                        {
                            var openId = json[i][OWNERID].Value<string>();
                            PerRequestDataContainer.OpenId = openId;
                        }
                    }
                    regids.Add(RegistJson(json[i], DynamicApiDataStoreRepository[0]));
                }
            }
            finally
            {
                if (IsContainerDynamicSeparation?.Value == true)
                {
                    if (IsVendor?.Value == true)
                    {
                        PerRequestDataContainer.VendorId = originalVendorId;
                        PerRequestDataContainer.SystemId = originalSystemId;
                    }
                    if (IsPerson?.Value == true)
                    {
                        PerRequestDataContainer.OpenId = originalOpenId;
                    }
                }
            }

            string result = json.Type != JTokenType.Array ? JToken.FromObject(regids[0]).ToString() : JToken.FromObject(regids).ToString();
            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string, Dictionary<string, string>>(this.MethodType.IsPost ? HttpStatusCode.Created : HttpStatusCode.OK, result, null));
        }

        private RegistReturnId RegistJson(JToken json, INewDynamicApiDataStoreRepository repository)
        {
            string registDocReturn = repository.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(json, this))?.Value;
            return new RegistReturnId { id = registDocReturn };
        }

        private void MergeAdminFields(JToken original, JToken json, IDictionary<string, object> admins)
        {
            var first = json.First;
            admins.ToList().ForEach(field =>
            {
                //管理項目がリクエストに無い場合のみ、振り直す
                if (!json.IsExistProperty(field.Key))
                {
                    first = json.First;
                    if (field.Key != REGDATE && field.Key != REGUSERID)
                    {
                        first.AddAfterSelf(new JProperty(field.Key, field.Value));
                    }
                }
            });

            var time = PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime();
            first = json.First;
            //管理項目がリクエストに無い場合のみ、振り直す
            if (!json.IsExistProperty(REGDATE))
            {
                first.AddAfterSelf(new JProperty(REGDATE, time));

            }

            if (!json.IsExistProperty(REGUSERID))
            {
                first.AddAfterSelf(new JProperty(REGUSERID, OpenId.Value));

            }

            if (!json.IsExistProperty(UPDDATE))
            {
                first.AddAfterSelf(new JProperty(UPDDATE, time));

            }

            if (!json.IsExistProperty(UPDUSERID))
            {
                first.AddAfterSelf(new JProperty(UPDUSERID, OpenId.Value));

            }
        }

        private IDictionary<string, object> CalcAdminFields(INewDynamicApiDataStoreRepository repository, JToken json)
        {
            var registerParam = ValueObjectUtil.Create<RegisterParam>(this, json);
            var result = new Dictionary<string, object>();
            DocumentDataId id;
            // idが無い場合のみ、割り振る
            if (!json.IsExistProperty("id"))
            {
                id = repository.KeyManagement.GetId(registerParam, repository.ResourceVersionRepository, PerRequestDataContainer);
                if (id != null)
                {
                    result.Add(ID, id.Id);
                }
            }

            if (IsVendor.Value)
            {
                result.Add(VENDORID, VendorId.Value);
                result.Add(SYSTEMID, SystemId.Value);
            }

            result.Add(REGDATE, PerRequestDataContainer.GetDateTimeUtil().LocalNow.ToUniversalTime());
            result.Add(REGUSERID, OpenId.Value);
            if ((IsOpenIdAuthentication.Value == true) || (IsOpenIdAuthentication.Value == false && string.IsNullOrEmpty(OpenId.Value) == false))
            {
                // 個人共有が許可されているか
                var isResourceSharingPerson = !string.IsNullOrEmpty(PerRequestDataContainer.XResourceSharingPerson) && this.ResourceSharingPersonRules.Any();
                result.Add(OWNERID, isResourceSharingPerson ? PerRequestDataContainer.XResourceSharingPerson : OpenId.Value);
            }
            return result;
        }
    }
}
