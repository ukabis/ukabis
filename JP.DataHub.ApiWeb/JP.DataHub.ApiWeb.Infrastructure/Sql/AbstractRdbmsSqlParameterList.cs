using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    abstract class AbstractRdbmsSqlParameterList : Dictionary<string, RdbmsSqlParameter>, IRdbmsSqlParameterList
    {
        protected object ParameterObjectCache = null;
        public virtual object AsParameterObject
        {
            get
            {
                if (ParameterObjectCache == null)
                {
                    ParameterObjectCache = this.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue);
                }

                return ParameterObjectCache;
            }
        }

        protected RepositoryType RepositoryType { get; }
        protected string AutoKeyPrefix { get; }


        public AbstractRdbmsSqlParameterList(RepositoryType repositoryType, string autoKeyPrefix)
        {
            RepositoryType = repositoryType;
            AutoKeyPrefix = autoKeyPrefix;
        }


        public virtual void Add(string key, object value, object mappingType = null, bool autoParameterName = true)
        {
            string parameterName;
            if (SqlBuilderUtil.IsSqlParameter(key, RepositoryType))
            {
                parameterName = SqlBuilderUtil.ToParameterName(key, RepositoryType);
            }
            else if (autoParameterName)
            {
                parameterName = $"{AutoKeyPrefix}_{(this.Count + 1)}".ToString();
            }
            else
            {
                parameterName = key;
            }

            base.Add(key, new RdbmsSqlParameter(parameterName, value, SqlBuilderUtil.ToSqlParameter(parameterName, RepositoryType), mappingType));
            ParameterObjectCache = null;
        }

        public virtual void AddRange(IDictionary<string, object> parameters, bool autoParameterName = true)
        {
            foreach (var parameter in parameters)
            {
                Add(parameter.Key, parameter.Value, null, autoParameterName);
            }
            ParameterObjectCache = null;
        }
    }
}
