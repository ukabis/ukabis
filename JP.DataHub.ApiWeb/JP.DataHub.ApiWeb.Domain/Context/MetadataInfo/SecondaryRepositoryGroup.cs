using System;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    /// セカンダリリポジトリグループ情報
    /// </summary>
    public class SecondaryRepositoryGroup
    {
        /// <summary>
        /// セカンダリリポジトリグループID
        /// </summary>
        public Guid SecondaryRepositoryId { get; set; }
        /// <summary>
        /// メソッドId
        /// </summary>
        public Guid MethodId { get; set; }
        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
