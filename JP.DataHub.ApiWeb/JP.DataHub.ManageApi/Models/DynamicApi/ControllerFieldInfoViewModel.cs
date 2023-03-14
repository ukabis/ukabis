﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public  class ControllerFieldInfoViewModel
    {
        public string ControllerFieldId { get; set; }

        public string ControllerId { get; set; }

        public string FieldId { get; set; }

        public string FieldName { get; set; }

        public bool IsActive { get; set; }
    }
}
