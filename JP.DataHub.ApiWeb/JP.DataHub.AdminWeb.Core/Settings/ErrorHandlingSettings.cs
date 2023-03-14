using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.Core.Settings
{
    public class ErrorHandlingSettings
    {
        public static readonly string SECTION_NAME = "ErrorHandling";
        public bool ShowDetails { get; set; }
    }
}
