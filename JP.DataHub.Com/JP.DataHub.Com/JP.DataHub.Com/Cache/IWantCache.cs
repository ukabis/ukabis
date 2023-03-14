namespace JP.DataHub.Com.Cache
{
    /// <summary>
    /// 対象のメソッドの結果をキャッシュするかどうかを判定するためのIF
    /// 判定はメソッドの実行後を想定
    /// </summary>
    public interface IWantCache
    {
        public bool IsCache();
    }
}
