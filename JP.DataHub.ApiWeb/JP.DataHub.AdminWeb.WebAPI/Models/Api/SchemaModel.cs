using JP.DataHub.AdminWeb.WebAPI.AnnotationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class SchemaModel
    {
        /// <summary>
        /// スキーマID
        /// </summary>
        public string SchemaId { get; set; }

        /// <summary>
        /// スキーマ名
        /// </summary>
        [Required(ErrorMessage = "モデル名は必須項目です。")]
        [MaxLengthEx(MaxLength = 100, ErrorMessageFormat = "モデル名は{0}文字以内で入力して下さい。")]
        public string SchemaName { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// JSONスキーマ
        /// </summary>
        [Required(ErrorMessage = "データモデルは必須項目です。")]
        public string JsonSchema { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdDate { get; set; }


        /// <summary>
        /// 新規かどうか(画面制御用)
        /// </summary>
        public bool IsNew { get; set; } = false;

        /// <summary>
        /// 作成された(画面制御用)
        /// </summary>
        public bool IsCreatedOrRenamed { get; set; } = false;


        #region プロパティ名相違補完
        //プロパティ名相違補完
        public string DataSchemaId { get => this.SchemaId; set => this.SchemaId = value; }

        public string DataSchema { get => this.JsonSchema; set => this.JsonSchema = value; }

        public string DataSchemaDescription { get => this.Description; set => this.Description = value; }

        public bool IsActive { get; set; }
        #endregion

        public SchemaModel()
        {

        }

        public SchemaModel(string vendorId)
        {
            VendorId = vendorId.ToLower();
        }

        public SchemaModel(string schemaId, string schemaName, string vendorId, string jsonSchema, string description, DateTime updDate)
        {
            SchemaId = schemaId.ToLower();
            SchemaName = schemaName;
            VendorId = vendorId.ToLower();
            JsonSchema = JsonSchema;
            Description = description;
            UpdDate = updDate;
        }
    }
}
