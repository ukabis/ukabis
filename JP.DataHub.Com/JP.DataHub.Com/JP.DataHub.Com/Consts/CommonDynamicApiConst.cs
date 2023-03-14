using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Consts
{
    public static class CommonDynamicApiConst
    {
        /// <summary>
        /// システム共通で使う場合のキー
        /// </summary>
        public static readonly string CommonKey = "CommonApiAccess";
        /// <summary>
        /// ALIAS（互換性のため）
        /// </summary>
        public static readonly string Key = CommonKey;

        /// <summary>
        /// ユーザーログインで使う場合のキー
        /// </summary>
        public static readonly string LoginUserKey = "LoginUserApiAccess";
    }
}
