using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace JP.DataHub.MVC.Extensions
{
    public static class ActionResultExtensions
    {
        public static ActionResult<List<T>> ToActionResult<T>(this List<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            else
            {
                return new ObjectResult(list) { StatusCode = (int)HttpStatusCode.OK };
            }
        }

        public static ActionResult<T> ToActionResult<T>(this T list)
        {
            if (list == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }
            else
            {
                return new ObjectResult(list) { StatusCode = (int)HttpStatusCode.OK };
            }
        }
    }
}
