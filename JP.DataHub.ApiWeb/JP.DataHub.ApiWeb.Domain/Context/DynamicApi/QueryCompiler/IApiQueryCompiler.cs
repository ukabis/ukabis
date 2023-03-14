﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.QueryCompiler
{
    // .NET6
    internal interface IApiQueryCompiler
    {
        Tuple<ApiQuery, RFC7807ProblemDetailExtendErrors> Compile(IDynamicApiAction action);
    }
}
