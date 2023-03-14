using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Shared
{
    internal class ProductCodeHelper
    {
        //管理番号の形式
        public enum NumberManagementMethod
        {
            Unknown,
            Serial = 21,
            Lot = 10,
            IndividualIdentificationNumber = 251,
        }

        public static string GetCode(NumberManagementMethod numberManagementMethod)
        {
            return ((int)numberManagementMethod).ToString();
        }

        //商品コードから形式を判定する
        public static NumberManagementMethod GetNumberManagementMethod(string productCode, string gtinCode)
        {
            if (string.IsNullOrEmpty(productCode) || string.IsNullOrEmpty(gtinCode))
            {
                return NumberManagementMethod.Unknown;
            }

            if (!gtinCode.StartsWith("01") && productCode.StartsWith("01"))
            {
                //GTINコードが01から始まってなくて、商品コードが01から始まってる場合は01を補う
                gtinCode = "01" + gtinCode;
            }

            //商品コードの後半部分を取り出す
            var productLatter = productCode?.ToString().Replace(gtinCode?.ToString(), "");
            if (productLatter.StartsWith(GetCode(NumberManagementMethod.Lot)))
            {
                return NumberManagementMethod.Lot;
            }
            if (productLatter.StartsWith(GetCode(NumberManagementMethod.Serial)))
            {
                return NumberManagementMethod.Serial;
            }
            if (productLatter.StartsWith(GetCode(NumberManagementMethod.IndividualIdentificationNumber)))
            {
                return NumberManagementMethod.IndividualIdentificationNumber;
            }

            return NumberManagementMethod.Unknown;
        }
    }
}
