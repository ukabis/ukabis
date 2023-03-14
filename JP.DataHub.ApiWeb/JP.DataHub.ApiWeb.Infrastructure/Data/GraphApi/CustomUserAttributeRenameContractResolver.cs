using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.GraphApi
{
    /// <summary>
    /// カスタムユーザー属性のプロパティ名をシリアライズ／デシリアライズ時に変換するContractResolverです。
    /// </summary>
    internal class CustomUserAttributeRenameContractResolver : DefaultContractResolver
    {
        private string name;

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="systemIdPropertyName">SystemIdのプロパティ名</param>
        public CustomUserAttributeRenameContractResolver(string systemIdPropertyName)
        {
            name = systemIdPropertyName;
        }

        /// <summary>
        /// JsonPropertyを作成します。
        /// </summary>
        /// <param name="member">MemberInfo</param>
        /// <param name="memberSerialization">MemberSerialization</param>
        /// <returns>JsonProperty</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            // プロパティ名がsystemIdの場合はカスタム属性の名前に変更する
            if (property.PropertyName == "systemId") property.PropertyName = name;
            return property;
        }
    }
}