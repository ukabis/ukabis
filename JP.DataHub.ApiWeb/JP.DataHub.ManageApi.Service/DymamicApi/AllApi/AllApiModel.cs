using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Serializer;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Actions.AllApi
{
    internal class AllApiModel
    {
        [Key(0)]
        public string vendor_id { get; set; }

        [Key(1)]
        public string system_id { get; set; }

        [Key(2)]
        public string controller_id { get; set; }
        [Key(3)]
        public string controller_description { get; set; }
        [Key(4)]
        public string controller_relative_url { get; set; }
        [Key(5)]
        public bool is_vendor { get; set; }
        [Key(6)]
        public bool is_enable_controller { get; set; }

        [Key(7)]
        public string api_id { get; set; }
        [Key(8)]
        public string api_description { get; set; }
        [Key(9)]
        public string method_name { get; set; }
        [Key(11)]
        public string method_type { get; set; }
        [Key(12)]
        public bool is_admin_authentication { get; set; }
        [Key(13)]
        public bool is_header_authentication { get; set; }
        [Key(14)]
        public bool is_openid_authentication { get; set; }
        [Key(15)]
        public string post_data_type { get; set; }
        [Key(16)]
        public string query { get; set; }
        [Key(17)]
        public bool is_enable_api { get; set; }
        [Key(18)]
        public string gateway_url { get; set; }
        [Key(19)]
        public string gateway_credential_username { get; set; }
        [Key(20)]
        public string gateway_credential_password { get; set; }
        [Key(21)]
        public bool is_over_partition { get; set; }
        [Key(22)]
        public string? repository_group_id { get; set; }
        [Key(23)]
        public List<AllApiRepositoryModel> all_repository_model_list { get; set; }

        [Key(24)]
        public string script { get; set; }
        [Key(25)]
        public string action_type_cd { get; set; }
        [Key(26)]
        public string script_type_cd { get; set; }
        [Key(27)]
        public int actiontype_version { get; set; }
        [Key(28)]
        public string repository_connection_string { get; set; }
        [Key(29)]
        public string repository_type_cd { get; set; }
        [Key(30)]
        public bool is_full { get; set; }
        [Key(31)]
        public bool is_hidden { get; set; }
        [Key(32)]
        public bool is_cache { get; set; }
        [Key(33)]
        public int cache_minute { get; set; }
        [Key(34)]
        public string cache_key { get; set; }
        [Key(35)]
        public bool is_accesskey { get; set; }

        [Key(36)]
        public bool is_automatic_id { get; set; }

        [Key(37)]
        public string partition_key { get; set; }
        [Key(38)]
        public string gateway_relay_header { get; set; }

        [Key(39)]
        public string? request_schema_id { get; set; }
        [Key(40)]
        public string request_schema { get; set; }
        [Key(41)]
        public string request_schema_name { get; set; }
        [Key(42)]
        public string? request_vendor_id { get; set; }
        [Key(43)]
        public DateTime request_reg_date { get; set; }
        [Key(44)]
        public DateTime request_upd_date { get; set; }
        [Key(45)]
        public bool request_is_active { get; set; }

        [Key(46)]
        public string? response_schema_id { get; set; }
        [Key(47)]
        public string response_schema { get; set; }
        [Key(48)]
        public string response_schema_name { get; set; }
        [Key(49)]
        public string? response_vendor_id { get; set; }
        [Key(50)]
        public DateTime response_reg_date { get; set; }
        [Key(51)]
        public DateTime response_upd_date { get; set; }
        [Key(52)]
        public bool response_is_active { get; set; }

        [Key(53)]
        public string? url_schema_id { get; set; }
        [Key(54)]
        public string url_schema { get; set; }
        [Key(55)]
        public string url_schema_name { get; set; }
        [Key(56)]
        public string? url_vendor_id { get; set; }
        [Key(57)]
        public DateTime url_reg_date { get; set; }
        [Key(58)]
        public DateTime url_upd_date { get; set; }
        [Key(59)]
        public bool url_is_active { get; set; }

        [Key(60)]
        public string alias_method_name { get; set; }
        [Key(61)]
        public bool is_nomatch_querystring { get; set; }
        [Key(62)]
        [MessagePackFormatter(typeof(TypeClassConvertFormatter))]
        public Type ActionInjector { get; set; }

        [Key(63)]
        public string? controller_schema_id { get; set; }
        [Key(65)]
        public string controller_schema { get; set; }
        [Key(66)]
        public DateTime controller_schema_reg_date { get; set; }
        [Key(67)]
        public DateTime controller_schema_upd_date { get; set; }
        [Key(68)]
        public bool controller_schema_is_active { get; set; }

        [Key(69)]
        public string controller_repository_key { get; set; }

        [Key(70)]
        public string? category_id { get; set; }
        [Key(71)]
        public string category_name { get; set; }
        [Key(72)]
        public bool is_toppage { get; set; }

        [Key(73)]
        public string controller_partition_key { get; set; }

        [Key(74)]
        public string controller_reg_username { get; set; }

        [Key(75)]
        public string controller_upd_username { get; set; }

        [Key(76)]
        public DateTime controller_reg_date { get; set; }

        [Key(77)]
        public DateTime controller_upd_date { get; set; }

        [Key(78)]
        public bool controller_is_active { get; set; }
        [Key(79)]
        public string api_reg_username { get; set; }

        [Key(80)]
        public string api_upd_username { get; set; }

        [Key(81)]
        public DateTime api_reg_date { get; set; }

        [Key(82)]
        public DateTime api_upd_date { get; set; }

        [Key(83)]
        public bool api_is_active { get; set; }

        [Key(84)]
        public string action_type_name { get; set; }

        [Key(85)]
        public bool is_transparent_api { get; set; }

        [Key(86)]
        List<AllApiCategoryModel> all_api_category_list { get; set; }
        [Key(87)]
        public bool is_vendor_system_authentication_allow_null { get; set; }

        [Key(88)]
        public string request_schema_description { get; set; }
        [Key(89)]
        public string response_schema_description { get; set; }
        [Key(90)]
        public string url_schema_description { get; set; }
        [Key(91)]
        public string controller_schema_description { get; set; }
        [Key(92)]
        public bool is_person { get; set; }
        [Key(93)]
        public string query_type_cd { get; set; }
        [Key(94)]
        public bool? is_enable_repository { get; set; }
        [Key(95)]
        public bool is_enable_attachfile { get; set; }
        [Key(96)]
        public AllApiRepositoryModel attachfile_blob_repository_model { get; set; }
        [Key(97)]
        public bool is_internal_call_only { get; set; }
        [Key(98)]
        public string internal_call_keyword { get; set; }
        [Key(99)]
        public bool is_skip_jsonschema_validation { get; set; }
        [Key(100)]
        public bool is_openid_authentication_allow_null { get; set; }
        [Key(101)]
        public string public_start_datetime { get; set; }
        [Key(102)]
        public string public_end_datetime { get; set; }
        [Key(103)]
        public bool is_use_blob_cache { get; set; }
        [Key(104)]
        public bool is_optimistic_concurrency { get; set; }
        [Key(105)]
        public bool is_enable_blockchain { get; set; }
        [Key(106)]
        public bool is_document_history { get; set; }
        [Key(107)]
        public AllApiRepositoryModel history_repository_model { get; set; }
        [Key(108)]
        public bool is_visible_agreement { get; set; }
        [Key(109)]
        public bool is_container_dynamic_separation { get; set; }
        [Key(110)]
        public bool is_clientcert_authentication { get; set; }
        [Key(111)]
        public bool is_otherresource_sqlaccess { get; set; }

        [Key(112)]
        public bool is_enable_resource_version { get; set; } = true;
    }
}
