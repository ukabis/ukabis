using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace JP.DataHub.Infrastructure.Core.Database
{
    public class CharacterLimit : ICharacterLimit
    {
		public string TablePrefix { get; set; } = "DB_";
		public string Namespace { get; set; } = "XXXX";

		/// <summary>
		/// データベース、テーブル、列から、文字の最大入力数を取得する
		/// </summary>
		/// <param name="key1">データベース</param>
		/// <param name="key2">テーブル</param>
		/// <param name="key3">列</param>
		/// <returns>最大入力文字数</returns>
		public int? GetMaxLength(string key1, string key2, string key3)
		{
			string className = $"{Namespace}.{key1}.{TablePrefix}{key2}";
			Type type = Type.GetType(className);
			if (type == null)
			{
				return null;
			}
			PropertyInfo[] properties = type.GetProperties();
			key3 = key3.ToLower();
			var p = properties.Where(x => x.Name.ToLower() == key3).FirstOrDefault();
			if (p == null)
			{
				return null;
			}

			var attr = p.GetCustomAttribute<MaxLengthAttribute>();
			if (attr == null)
			{
				return null;
			}
			return attr.Length;
		}

		public IEnumerable<Attribute> GetCustomAttribute(string key1, string key2, string key3)
		{
			string className = $"{Namespace}.{key1}.{TablePrefix}{key2}";
			Type type = Type.GetType(className);
			if (type == null)
			{
				return null;
			}
			PropertyInfo[] properties = type.GetProperties();
			key3 = key3.ToLower();
			var p = properties.Where(x => x.Name.ToLower() == key3).FirstOrDefault();
			if (p == null)
			{
				return null;
			}
			return p.GetCustomAttributes();
		}
	}
}
