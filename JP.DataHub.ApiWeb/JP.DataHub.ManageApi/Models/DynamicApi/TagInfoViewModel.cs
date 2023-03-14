﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class TagInfoViewModel
    {
        public string TagId { get; set; }

        public string TagCode1 { get; set; }

        public string TagCode2 { get; set; }

        public string TagName { get; set; }

        public string ParentTagId { get; set; }

        public List<TagInfoViewModel> Children { get; set; }
    }
}
