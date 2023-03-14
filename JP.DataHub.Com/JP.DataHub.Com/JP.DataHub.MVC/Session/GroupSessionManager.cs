using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Session;
using JP.DataHub.MVC.Session.Models;

namespace JP.DataHub.MVC.Session
{
    public class GroupSessionManager : UnityAutoInjection, IGroupSessionManager
    {
        private const string SESSION_GROUP_KEY = "ownGroup";

        [Dependency]
        public IHttpContextAccessor HttpContextAccessor;

        public string CraeteGroupQueryParam(string queryString = null)
        {
            var groupId = HttpContextAccessor.HttpContext.Session.GetObject<string>(SESSION_GROUP_KEY);
            var groupParam = groupId == Guid.Empty.ToString() ? null : $"groupId={groupId}";
            return groupParam;
        }

        public GroupModel Get() => HttpContextAccessor.HttpContext.Session.GetObject<GroupModel>(SESSION_GROUP_KEY);

        public void Remove() => HttpContextAccessor.HttpContext.Session.Remove(SESSION_GROUP_KEY);

        public void Save(GroupModel value) => HttpContextAccessor.HttpContext.Session.SetObject<GroupModel>(SESSION_GROUP_KEY, value);

        public bool IsGroupSession() => HttpContextAccessor.HttpContext.Session.TryGetValue(SESSION_GROUP_KEY, out _);
    }
}
