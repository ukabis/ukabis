using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class SampleCodeModel
    {
        /// <summary>
        /// サンプルコードID
        /// </summary>
        public string SampleCodeId { get; set; }

        /// <summary>
        /// 言語ID
        /// </summary>
        public string LanguageId { get; set; }

        /// <summary>
        /// 言語
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// コード
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// アクティブか
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
