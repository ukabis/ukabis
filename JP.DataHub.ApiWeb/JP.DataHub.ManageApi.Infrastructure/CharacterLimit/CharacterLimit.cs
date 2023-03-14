using JP.DataHub.ManageApi.Service.CharacterLimit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Infrastructure.CharacterLimit
{
    public class CharacterLimit : ICharacterLimit
    {
        private const string _assemblyName = "JP.DataHub.Infrastructure.Database";

        /// <summary>
        /// データベース、テーブル、列から、文字の最大入力数を取得する
        /// </summary>
        /// <param name="key1">データベース</param>
        /// <param name="key2">テーブル</param>
        /// <param name="key3">列</param>
        /// <returns>最大入力文字数</returns>
        public int? GetMaxLength(string key1, string key2, string key3)
        {
            string className = string.Format($"JP.DataHub.Infrastructure.Database.{key1}.DB_{key2}");
            Type type = Type.GetType($"{className}, {_assemblyName}");
            if (type == null)
            {
                throw new Exception("MaxLength not found ClassName");
            }

            PropertyInfo[] properties = type.GetProperties();
            key3 = key3.ToLower();
            var p = properties.Where(x => x.Name.ToLower() == key3).FirstOrDefault();
            if (p == null)
            {
                throw new Exception("MaxLength not found PropertyName");
            }

            var attr = p.CustomAttributes.Where(x => x.AttributeType.Name.ToString() == "MaxLengthAttribute").FirstOrDefault();
            if (attr == null)
            {
                throw new Exception("MaxLength not found MaxLengthAttributes");
            }
            var ca = attr.ConstructorArguments[0];
            if (ca.Value is int)
            {
                return (int)ca.Value;
            }
            return null;
        }
    }
}
