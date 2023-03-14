using JP.DataHub.MvcTestApp.Models;
using JP.DataHub.MVC.Session;
using JP.DataHub.Com.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace JP.DataHub.MvcTestApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private ISession Session { get => HttpContextAccessor.HttpContext.Session; }

        [Session]
        public string val1 { get; set; }
        [Session]
        public int val2 { get; set; }
        [Session]
        public Member Member3 { get; set; } = new Member() { Name = "masas", Age = 55 };

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            val1 = "123";
            val2 = 123;
            _logger = logger;
            HttpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            // Simpleな情報をセッションに保存する
            Session.SetString("test", "123");
            Session.To("test2", "456");

            // ページのプロパティのセッション属性があれば、それを自動で保存する
            Session.Store(this);

            return View();
        }

        public IActionResult Privacy()
        {
            var tmp = Session.GetString("test");
            tmp = Session.Get<string>("test2");

            val1 = "456";
            val2 = 456;
            Member3.Name = Guid.NewGuid().ToString();
            Member3.Age = 5;
            // 復元すると、val1とval2が保存したものが復元される
            Session.Restore(this);

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class Member
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
