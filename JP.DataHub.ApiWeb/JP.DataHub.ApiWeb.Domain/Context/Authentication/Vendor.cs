using MessagePack;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    [MessagePackObject]
    internal class Vendor : IEntity
    {
        [IgnoreMember]
        protected IVendorRepository VendorRepository => UnityCore.Resolve<IVendorRepository>();
        [IgnoreMember]
        protected ICache Cache => UnityCore.Resolve<ICache>();
        [IgnoreMember]
        protected ICacheManager CacheManager => UnityCore.Resolve<ICacheManager>();

        [Key(0)]
        public VendorId VendorId { get; set; }

        [Key(1)]
        public VendorName VendorName { get; set; }

        [Key(2)]
        public IsEnable IsEnable { get; set; }

        [Key(3)]
        public IsDataOffer IsDataOffer { get; set; }

        [Key(4)]
        public IsDataUse IsDataUse { get; set; }

        [Key(5)]
        public List<SystemEntity> SystemList { get; set; }

        [Key(6)]
        public List<StaffRole> StaffList { get; set; }

        [Key(7)]
        public FunctionNames ApiFunction { get; set; }

        [Key(8)]
        public List<VendorLinkAdmin> VendorLinkList { get; set; }

        [Key(9)]
        public AttachFileStorageId AttachFileStorageId { get; set; }

        [Key(10)]
        public UpdDate UpdDate { get; set; }

        /// <summary>
        /// OpenID認証局リスト
        /// </summary>
        [Key(11)]
        public List<OpenIdCa> OpenIdCaList { get; set; }

        [Key(12)]
        public List<EmailAddress> RepresentativeMailAddress { get; set; }

        private Vendor(VendorId vendorId, VendorName vendorName, IsDataOffer isDataOffer, IsDataUse isDataUse, IsEnable isEnable,
                      List<SystemEntity> systemList = null, List<StaffRole> staffList = null, List<VendorLinkAdmin> vendorLinkList = null,
                      AttachFileStorageId attachFileStorageId = null, UpdDate updDate = null, List<OpenIdCa> openIdCaList = null, List<EmailAddress> emailAddressList = null)
        {
            if (string.IsNullOrEmpty(vendorId?.Value))
            {
                vendorId = new VendorId(Guid.NewGuid().ToString());
            }

            VendorId = vendorId;
            VendorName = vendorName;
            IsDataOffer = isDataOffer;
            IsDataUse = isDataUse;
            IsEnable = isEnable;
            UpdDate = updDate;
            SystemList = systemList;
            StaffList = staffList;
            VendorLinkList = vendorLinkList;
            AttachFileStorageId = attachFileStorageId;
            OpenIdCaList = openIdCaList;
            RepresentativeMailAddress = emailAddressList;
        }

        public static Vendor Create(VendorId vendorId, VendorName vendorName, IsDataOffer isDataOffer, IsDataUse isDataUse, IsEnable isEnable,
                                    List<SystemEntity> systemList = null, List<StaffRole> staffList = null, List<VendorLinkAdmin> vendorLinkList = null,
                                    AttachFileStorageId attachFileStorageId = null, List<OpenIdCa> openIdCaList = null, UpdDate updDate = null, List<EmailAddress> emailAddressList = null)
        {
            return new Vendor(vendorId, vendorName, isDataOffer, isDataUse, isEnable, systemList, staffList, vendorLinkList, attachFileStorageId, updDate, openIdCaList, emailAddressList);
        }


        public void Remove()
        {
            //this.VendorRepository.Delete(VendorId);
            //CacheManager.Fire(Cache, "vendor");
            //CacheManager.FireId(Cache, "vendor_id", VendorId.Value);
        }

        public void Update()
        {
            //this.VendorRepository.Update(this);
            //CacheManager.Fire(Cache, "vendor");
            //CacheManager.FireId(Cache, "vendor_id", VendorId.Value);
        }

        public void RegisterNotified()
        {
            //this.VendorRepository.Registration(this);
            //CacheManager.Fire(Cache, "vendor");
            //CacheManager.FireId(Cache, "vendor_id", VendorId.Value);
        }

        #region 管理画面以外の処理

        // MessagePackObjectのために用意
        public Vendor()
        {
        }

        public void Authorization(ApiRelativeUrl apiGroupName)
        {
            if (ApiFunction == null || ApiFunction.Value.Count() == 0)
            {
                throw new AccessDeniedException(string.Format("{0} API", apiGroupName.Value));
            }
            bool hit = false;
            for (int i = 0; i < apiGroupName.Split.Count; i++)
            {
                string rel1 = string.Join("/", apiGroupName.Split.GetRange(0, i + 1));
                string rel2 = rel1 + "/*";
                if (ApiFunction.Value.Contains(rel1) == true || ApiFunction.Value.Contains(rel2) == true)
                {
                    hit = true;
                    break;
                }
            }
            if (hit == false)
            {
                throw new AccessDeniedException(string.Format("{0} API", apiGroupName.Value));
            }
        }

        public void AddStaff(StaffRole staffRole)
        {
            //VendorRepository.AddStaff(this, staffRole);
            //CacheManager.Fire(Cache, "vendor");
            //CacheManager.FireId(Cache, "vendor_id", VendorId.Value);
        }

        #endregion
    }
}
