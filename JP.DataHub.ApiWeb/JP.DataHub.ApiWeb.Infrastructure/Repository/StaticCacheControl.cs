using JP.DataHub.Com.Log;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    internal class StaticCacheControl<T>
    {
        private StaticCache<T> ActiveCache => toggle ? _cache1 : _cache2;
        private StaticCache<T> StandByCache => toggle ? _cache2 : _cache1;

        private bool toggle = true;
        private StaticCache<T> _cache1 = new StaticCache<T>();
        private StaticCache<T> _cache2 = new StaticCache<T>();

        private JPDataHubLogger _logger = new JPDataHubLogger(typeof(StaticCacheControl<T>));
        private object lockObj = new object();

        public void Refresh(Func<T> getFullData)
        {
            var acquiredLock = false;
            try
            {
                // ロックが取得できない(=リフレッシュ中)なら何もしない
                Monitor.TryEnter(lockObj, 0, ref acquiredLock);
                if (acquiredLock)
                {
                    // スタンバイ側を更新
                    var cache = StandByCache;
                    cache.Data = getFullData();

                    // アクティブとスタンバイを入れ替え
                    toggle = !toggle;
                }
                else
                {
                    _logger.Warn("Static cache is on refreshing. Additional refresh requests will be ignored until the refresh is complete.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected errors occured on static cache refresh", ex);
            }
            finally
            {
                if (acquiredLock)
                {
                    Monitor.Exit(lockObj);
                }
            }
        }

        public bool TryGet(out T data)
        {
            var cache = ActiveCache;
            if (cache.Data == null)
            {
                data = default(T);
                return false;
            }
            else
            {
                data = cache.Data;
                return true;
            }
        }


        private class StaticCache<T>
        {
            public T Data { get; set; }
        }
    }
}
