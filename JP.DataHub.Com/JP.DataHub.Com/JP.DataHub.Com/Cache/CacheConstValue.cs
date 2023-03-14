using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Cache
{
    public class CacheConstValue
    {
        /// <summary>
        /// The config section.
        /// </summary>
        public const string ConfigSection = "cache";

        /// <summary>
        /// defaultのセクション
        /// </summary>
        public const string ConfigDefaultSection = "default";

        /// <summary>
        /// InMemoryMode
        /// </summary>
        public const string InMemoryMode = "InMemoryMode";

        /// <summary>
        /// モードのセクション
        /// </summary>
        public const string ConfigModeSection = "mode";

        /// <summary>
        /// キャッシュ最大サイズ(1件あたり)のセクション
        /// </summary>
        public const string ConfigMaxSizeSection = "maxsize";

        /// <summary>
        /// オプションのセクション
        /// </summary>
        public const string ConfigOptionSection = "options";
        /// <summary>
        /// Redis
        /// </summary>
        public const string ConfigModeRedis = "redis";
        /// <summary>
        /// inMemory
        /// </summary>
        public const string ConfigModeInMomery = "inmemory";
        /// <summary>
        /// none
        /// </summary>
        public const string ConfigModeInNone = "none";
        /// <summary>
        /// blob
        /// </summary>
        public const string ConfigModeInBlob = "blob";
        /// <summary>
        /// ProfiledCacheの子要素のPostfix
        /// </summary>
        public const string ProfiledCacheChildPostfix = "Composite";
        /// <summary>
        /// LifetimeManager
        /// </summary>
        public const string ConfigLifetimeManagerSection = "lifetimeManager";
        /// <summary>
        /// cache expiration
        /// </summary>
        public const string ConfigExpiration = "Expiration";
        /// <summary>
        /// ConfigConnectionStrings
        /// </summary>
        public const string ConfigConnectionStrings = "ConnectionStrings";

        /// <summary>
        /// キャッシュ無効化不可
        /// </summary>
        public const string ConfigImmutable = "Immutable";
    }
}
