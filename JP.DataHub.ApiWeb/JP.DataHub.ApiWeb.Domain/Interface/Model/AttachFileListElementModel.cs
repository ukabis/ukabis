using JP.DataHub.Com.DataAnnotations;

namespace JP.DataHub.ApiWeb.Domain.Interface.Model
{
    public class AttachFileListElementModel : AbstractArgumentModel
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 登録日時
        /// </summary>
        public string RegisterDateTime { get; set; }

        /// <summary>
        /// 登録者ID
        /// </summary>
        public string RegisterUserId { get; set; }
    }
}