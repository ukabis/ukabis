﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Validations
{
    public interface IValidationAttributeEx
    {
        Type ExceptionType { get; }
    }
}
