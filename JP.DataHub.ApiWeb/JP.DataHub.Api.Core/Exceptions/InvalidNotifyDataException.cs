﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Exceptions
{
    [Serializable]
    public class InvalidNotifyDataException : Exception
    {
        public override string Message { get; }

        public InvalidNotifyDataException(string message)
        {
            Message = message;
        }
    }
}
