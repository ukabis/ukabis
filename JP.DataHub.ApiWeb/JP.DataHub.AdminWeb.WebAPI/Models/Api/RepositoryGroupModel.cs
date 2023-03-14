using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class RepositoryGroupModel
    {
        public string RepositoryGroupId { get; set; }

        public string RepositoryGroupName { get; set; }

        public string RepositoryTypeCd { get; set; }

        public string RepositoryTypeName { get; set; }

        public int SortNo { get; set; }

        public bool IsDefault { get; set; }

        public bool? IsEnable { get; set; }

        public IEnumerable<RepositoryGroupVendorModel> VendorList { get; set; }

        public List<PhysicalRepositoryModel> PhysicalRepositoryList { get; set; }

        /// <summary>
        /// 添付ファイルストレージか
        /// </summary>
        public bool IsAttachFileBlobStorage
        {
            get
            {
                return (RepositoryTypeCd == "afb");
            }
        }

        /// <summary>
        /// 添付ファイルメタストレージか
        /// </summary>
        public bool IsAttachFileMetaStorage
        {
            get
            {
                return (RepositoryTypeCd == "afm" || RepositoryTypeCd == "afs" || RepositoryTypeCd == "ora");
            }
        }
        /// <summary>
        /// ブロックチェーンノードか
        /// </summary>
        public bool IsBlockchainNode
        {
            get
            {
                return (RepositoryTypeCd == "bcn");
            }
        }
        /// <summary>
        /// 履歴用ストレージか
        /// </summary>
        public bool IsHistoryStorage
        {
            get
            {
                return (RepositoryTypeCd == "dhs");
            }
        }
    }

    public class RepositoryGroupVendorModel
    {
        public string VendorId { get; set; }

        public bool IsUsed { get; set; }
    }
}

