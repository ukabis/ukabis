﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Configuration
{
    public class VendorAuthenticationJwtKeyConfig
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public string AudienceSecret { get; set; }
    }
}
