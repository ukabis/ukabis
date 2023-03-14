using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Consts
{
    public static class RegexConsts
    {
        public static readonly string CheckTypeGuidRegex = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
        public static readonly string CheckZipCodeRegex = "\\d{7}";
    }
}
