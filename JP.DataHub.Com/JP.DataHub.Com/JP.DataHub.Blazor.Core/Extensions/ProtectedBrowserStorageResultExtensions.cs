using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace JP.DataHub.Blazor.Core.Extensions
{
    public static class ProtectedBrowserStorageResultExtensions
    {
        public static ProtectedBrowserStorageResult<TResult> IsFail<TResult>(this ProtectedBrowserStorageResult<TResult> task, Func<ProtectedBrowserStorageResult<TResult>, string> func)
        {
            if (task.Success == false && func != null)
            {
                var msg = func(task);
                throw new Exception(msg);
            }
            return task;
        }
    }
}
