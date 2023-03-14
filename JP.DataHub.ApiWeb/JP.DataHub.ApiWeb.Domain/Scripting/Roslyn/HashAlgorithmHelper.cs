using System.Text;
using JP.DataHub.Com.HashAlgorithm;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// 文字列のHash化を行うヘルパークラスです。
    /// </summary>
    [RoslynScriptHelp]
    public class HashAlgorithmHelper
    {
        /// <summary>
        /// 文字列をHash化します。
        /// </summary>
        /// <param name="value">
        /// Hash化する文字列
        /// </param>
        /// <param name="hashAlgorithm">
        /// Hashアルゴリズム、sha-256、sha-384、sha-512のいずれか指定無しでSha256
        /// </param>
        /// <returns>
        /// Hash
        /// </returns>
        public static string ComputeHash(string value, string hashAlgorithm = "sha-384")
        {
            var hashAlgorithmType = (HashAlgorithmType)Enum.Parse(typeof(HashAlgorithmType), hashAlgorithm.Replace("-", ""), true);
            var hash = HashCalculation.ComputeHash(Encoding.UTF8.GetBytes(value), hashAlgorithmType);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
