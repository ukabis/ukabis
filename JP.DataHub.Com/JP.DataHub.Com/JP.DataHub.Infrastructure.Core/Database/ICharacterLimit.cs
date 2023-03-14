using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Infrastructure.Core.Database
{
    public interface ICharacterLimit
    {
        string TablePrefix { get; set; }
        string Namespace { get; set; }

        /// <summary>
        /// データベース、テーブル、列から、文字の最大入力数を取得する
        /// </summary>
        /// <param name="key1">データベース</param>
        /// <param name="key2">テーブル</param>
        /// <param name="key3">列</param>
        /// <returns>最大入力文字数</returns>
		int? GetMaxLength(string key1, string key2, string key3);

        /// <summary>
        /// データベース、テーブル、列から、該当するプロパティの属性を取得する
        /// </summary>
        /// <param name="key1">データベース</param>
        /// <param name="key2">テーブル</param>
        /// <param name="key3">列</param>
        /// <returns>カスタム属性の配列</returns>
        IEnumerable<Attribute> GetCustomAttribute(string key1, string key2, string key3);
    }
}
