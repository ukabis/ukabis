using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Oracle;
using static Dapper.SqlMapper;
using static Dapper.Oracle.OracleDynamicParameters;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.Com.Sql
{
    public static class IDynamicParametersExtension
    {
        public static IDynamicParameters AddDynamicParams(this IDynamicParameters self, dynamic param)
        {
            if (self is OracleDynamicParameters ora)
            {
                ora.BindByName = true;
                return AddOracleParams(ora, param);
            }
            else
            {
                return (self as DynamicParameters)?.AddDynamicParams(param);
            }
        }

        private static IDynamicParameters AddOracleParams(OracleDynamicParameters self, dynamic param)
        {
            if (param is object obj)
            {
                if (!(obj is OracleDynamicParameters subDynamic))
                {
                    if (!(obj is IEnumerable<KeyValuePair<string, object>> dictionary))
                    {
                        foreach (var kvp in obj.ObjectToDictionary())
                        {
                            if (kvp.Value is Guid)
                            {
                                self.Add(kvp.Key, kvp.Value.ToString());
                            }
                            else
                            {
                                self.Add(kvp.Key, kvp.Value);
                            }
                        }
                    }
                    else
                    {
                        foreach (var kvp in dictionary)
                        {
                            if (kvp.Value is Guid)
                            {
                                self.Add(kvp.Key, kvp.Value.ToString());
                            }
                            else
                            {
                                self.Add(kvp.Key, kvp.Value);
                            }
                        }
                    }
                }
                else
                {
                    var parameters = subDynamic.GetParameters();
                    if (parameters != null)
                    {
                        foreach (var kvp in parameters)
                        {
                            parameters.Add(kvp.Key, kvp.Value);
                            if (kvp.Value.Value is Guid)
                            {
                                kvp.Value.Value = kvp.Value.Value.ToString();
                                parameters.Add(kvp.Key, kvp.Value);
                            }
                            else
                            {
                                parameters.Add(kvp.Key, kvp.Value);
                            }
                        }
                    }

                    var templates = subDynamic.GetTemplates();
                    if (templates != null)
                    {
                        foreach (var t in templates)
                        {
                            templates.Add(t);
                        }
                    }
                }
            }

            return self;
        }

        private static Dictionary<string, OracleParameterInfo>? GetParameters(this OracleDynamicParameters self)
        {
            var fieldInfo = typeof(OracleDynamicParameters).GetProperty("Parameters", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldInfo?.GetValue(self) as Dictionary<string, OracleParameterInfo>;
        }

        private static List<object>? GetTemplates(this OracleDynamicParameters self)
        {
            var fieldInfo = typeof(OracleDynamicParameters).GetField("templates", BindingFlags.Instance | BindingFlags.NonPublic);
            return fieldInfo?.GetValue(self) as List<object>;
        }

        private static string Clean(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                switch (name[0])
                {
                    case '@':
                    case ':':
                    case '?':
                        return name.Substring(1);
                }
            }

            return name;
        }

        private static OracleParameterInfo GetParamInfo(OracleDynamicParameters self, string name)
        {
            var parameters = self.GetParameters();

            if (parameters != null)
            {
                var cleanName = Clean(name);
                if (parameters.ContainsKey(cleanName))
                {
                    return parameters[cleanName];
                }
            }

            throw new NotSupportedException($"バインド変数 '{name}' は存在しません");
        }

        private static void Remove(this OracleDynamicParameters self, string name)
        {
            var parameters = self.GetParameters();
            if (parameters == null) return;
            parameters.Remove(Clean(name));
        }

        public static IDynamicParameters SetDbType(this IDynamicParameters self, string name, OracleMappingType type)
        {
            if (self is OracleDynamicParameters ora)
            {
                GetParamInfo(ora, name).DbType = type;
            }
            return self;
        }

        public static IDynamicParameters SetNClob(this IDynamicParameters self, string name)
        {
            self.SetDbType(name, OracleMappingType.NClob);
            return self;
        }

        public static IDynamicParameters SetArray(this IDynamicParameters self, string name, OracleMappingType type)
        {
            if (self is OracleDynamicParameters ora)
            {
                var info = GetParamInfo(ora, name);
                info.DbType = type;
                info.CollectionType = OracleMappingCollectionType.PLSQLAssociativeArray;
                if (info.Value is IList arr)
                {
                    info.Size = arr.Count;
                }
                else
                {
                    throw new InvalidOperationException($"バインド変数 '{name}' は配列ではありません");
                }
            }
            return self;
        }
    }
}
