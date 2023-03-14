using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class CertificationApplyModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertificationApplyId { get; set; }
        public string CertificationQuestionnaireId { get; set; }
        public string SubmitOpenId { get; set; }
        public DateTime EntryDate { get; set; }
        public CertificationApply_ApplyModel Apply { get; set; }
        public string ApplyDate { get; set; }
        public List<CertificationApply_AnswerModel> Answer { get; set; }
        public bool IsAutoPublic { get; set; }
        public string CertificationApplyStatusCode { get; set; }
        public bool IsAutomaticJudgment { get; set; }
        public int TotalScore { get; set; }
        public int ThresholdScore { get; set; }
        public bool IsPass { get; set; }
        public string DisqualificationReason { get; set; }
        public DateTime JudgementDate { get; set; }
        public string ExpirationDate { get; set; }
        public bool IsTemporaryCertificationIssue { get; set; }
        public bool IsCertificationIssue { get; set; }
    }

    public class CertificationApply_ApplyModel
    {
        public string CertificationApplyTypeCode { get; set; }
        public string Company { get; set; }
        public string Office { get; set; }
        public string Applicanter { get; set; }
    }

    public class CertificationApply_AnswerModel
    {
        public string CertificationQuestionnaireId { get; set; }
        public string CertificationQuestionnaireClassificationId { get; set; }
        public string CertificationQuestionnaireQuestionId { get; set; }
        public List<string> Choice { get; set; }
        public string Text { get; set; }
        public List<CertificationApply_AttachFileModel> AttachFile { get; set; }
        public int Score { get; set; }
    }

    public class CertificationApply_AttachFileModel
    {
        public string FileId { get; set; }
        public string ImageMimeType { get; set; }
        public List<CertificationApply_MetaModel> Meta { get; set; }
    }

    public class CertificationApply_MetaModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    
}
