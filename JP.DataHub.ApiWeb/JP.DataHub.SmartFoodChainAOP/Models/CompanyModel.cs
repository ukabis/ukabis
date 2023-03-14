using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
    public class CompanyModel
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<NameLangModel> CompanyNameLang { get; set; }
        public string CompanyNameKana { get; set; }
        public string IndustoryTypeCode { get; set; }
        public string ZipCode { get; set; }
        public string Address1 { get; set; }
        public List<AddressLangModel> Address1Lang { get; set; }
        public string Address2 { get; set; }
        public List<AddressLangModel> Address2Lang { get; set; }
        public string Address3 { get; set; }
        public List<AddressLangModel> Address3Lang { get; set; }
        public string AddressJoin { get; set; }
        public CeoModel Ceo { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string HomepageUrl { get; set; }
        public string MailAddress { get; set; }
        public List<ImageModel> Images { get; set; }
        public string GlnCode { get; set; }
        public string GS1CompanyCode { get; set; }
        public string CountryCode { get; set; }
        public string TermsOfSettlement { get; set; }
        public AgreachModel Agreach { get; set; }
        public CompanyProducerModel Producer { get; set; }
        public CompanyBuyerModel Buyer { get; set; }
        public string GroupId { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class AgreachModel
    {
        public bool IsImport { get; set; }
        public string ProducerId { get; set; }
        public string ActualconsId { get; set; }
        public string WholesaleId { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class CompanyProducerModel
    {
        public List<Items> ProductionItem { get; set; }
        public List<ProductionScale> ProductionScale { get; set; }
        public int NumberOfEmployees { get; set; }
        public List<ProducerSns> ProducerSns { get; set; }
        public string ProductionPolicy { get; set; }
        public List<string> Keyword { get; set; }
        public string TermsOfSettlement { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class CompanyBuyerModel
    {
        public int BranchCount { get; set; }
        public string PurchasePolicy { get; set; }
        public string RequestToSuppliers { get; set; }
        public string ClosingdatePayment { get; set; }
        public int DesignatedWholesalers { get; set; }
        public List<string> TrWhoolesaleMarket { get; set; }
        public int TransactionForm { get; set; }
        public List<BuyerCommissionRate> CommissionRate { get; set; }
        public int DeliveryPlace { get; set; }
        public int LogisticsArrangement { get; set; }
        public List<ProducerSns> ProducerSns { get; set; }
        public List<string> Keyword { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class Items
    {
        public List<string> MajorClassName { get; set; }
        public List<string> MediumClassName { get; set; }
        public List<string> SmallClassName { get; set; }
        public List<string> Freeword { get; set; }
        public List<string> DisplayName { get; set; }
        public List<string> MajorDisplayName { get; set; }
        public List<string> MediumDisplayName { get; set; }
        public List<string> SmallDisplayName { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class ProductionScale
    {
        public long PlantingArea { get; set; }
        public long TotalProduction { get; set; }
        public long AmountOfSales { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class ProducerSns
    {
        public string Name { get; set; }
        public string Detail { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class CeoModel
    {
        public string CeoName { get; set; }
        public string CeoNameKana { get; set; }
        public string CeoNameMailAddress { get; set; }
        public List<NameLangModel> CeoLang { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class ImageModel
    {
        public string ImagePath { get; set; }
        public string ImageDescription { get; set; }
        public bool DefaultImageFlag { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class BuyerCommissionRate
    {
        public int rate { get; set; }
        public int other { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class NameLangModel
    {
        public string Name { get; set; }
        public string LocaleCode { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class AddressLangModel
    {
        public string Address { get; set; }
        public string LocaleCode { get; set; }
    }
}
