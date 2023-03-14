using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.MVC.Unity
{
    public class UnityAutoInjectionController : ControllerBase
    {
        public UnityAutoInjectionController()
        {
            this.AutoInjection();
        }
    }
}
