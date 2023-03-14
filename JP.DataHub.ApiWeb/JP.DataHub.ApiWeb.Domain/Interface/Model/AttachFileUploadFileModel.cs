using JP.DataHub.Com.DataAnnotations;

namespace JP.DataHub.ApiWeb.Domain.Interface.Model
{
    public class AttachFileUploadFileModel : AbstractArgumentModel
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// 入力ファイルストリーム
        /// </summary>
        public Stream InputStream { get; set; }
    }
}