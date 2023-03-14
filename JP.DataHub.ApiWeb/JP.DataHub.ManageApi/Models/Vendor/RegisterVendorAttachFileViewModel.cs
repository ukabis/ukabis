using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.Com.Validations.Annotations;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class RegisterVendorAttachFileViewModel
    {

        /// <summary>
        /// ベンダーID
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string VendorId { get; set; }


        /// <summary>
        /// ベンダーID
        /// </summary>
        [RequiredGuid(ErrorMessage = "必須項目です。")]
        public string AttachFileStorageId { get; set; }
    }
}
