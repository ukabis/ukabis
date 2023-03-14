using System.Collections.Generic;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class CertificationQuestionnaireModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertificationQuestionnaireId { get; set; }
        public string CertificationNameId { get; set; }
        public string AccreditationId { get; set; }
        public CertificationQuestionnaire_VersionModel Version { get; set; }
        public bool IsActive { get; set; }
        public CertificationQuestionnaire_AutomaticJudgmentModel AutomaticJudgment { get; set; }
        public CertificationQuestionnaire_IndividualQuestionModel Indivisual { get; set; }
        public bool IsAutoPublic { get; set; }
        public List<CertificationQuestionnaire_QuestionnaireAdditionalItemModel> AddtionalCompanyItem { get; set; }
        public List<CertificationQuestionnaire_QuestionnaireAdditionalItemModel> AddtionalOfficeItem { get; set; }
        public CertificationQuestionnaire_ExpirationModel Expiration { get; set; }
        public bool IsTemporaryCertificate { get; set; }
        public List<string> ApplicantUnitCode { get; set; }
        public List<CertificationQuestionnaire_PageStructureModel> PageStructure { get; set; }
        public string TemporaryCertificationDataId { get; set; }
    }

    public class CertificationQuestionnaire_VersionModel
    {
        public int VersionNo { get; set; }
        public string PublicEndDate { get; set; }
    }

    public class CertificationQuestionnaire_AutomaticJudgmentModel
    {
        public bool IsAutomaticJudgment { get; set; }
        public int PassScore { get; set; }
    }

    public class CertificationQuestionnaire_IndividualQuestionModel
    {
        public bool IsIndividualQuestion { get; set; }
        public string IndivisualTitleName { get; set; }
        public List<string> IndivisualSelectionItems { get; set; }
    }

    public class CertificationQuestionnaire_QuestionnaireAdditionalItemModel
    {
        public string AdditionalItenName { get; set; }
        public bool IsRequired { get; set; }
        public int MinLength { get; set; }
        public int MaxLength { get; set; }
        public string RegularExpression { get; set; }
        public string RoslynScript { get; set; }
    }

    public class CertificationQuestionnaire_ExpirationModel
    {
        public int Day { get; set; }
        public int Week { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class CertificationQuestionnaire_PageStructureModel
    {
        public string PageTitleName { get; set; }
        public string PageTypeCode { get; set; }
        public string InsertionHtml { get; set; }
        public string CancelButtonName { get; set; }
        public string NextButtonName { get; set; }
        public string PrevButtonName { get; set; }
        public string PageTypeDetail { get; set; }
    }
}
