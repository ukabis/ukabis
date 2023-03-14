using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.MVC.Session.Models;

namespace JP.DataHub.MVC.Session
{
    public interface IGroupSessionManager
    {
        string CraeteGroupQueryParam(string queryString = null);

        GroupModel Get();

        void Remove();

        void Save(GroupModel value);

        bool IsGroupSession();
    }
}
