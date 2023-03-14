using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    internal record FunctionNode : IValueObject
    {
        public string FunctionName { get; }

        public string AbsoluteUrl { get; }

        public string RelativeUrl { get; }

        public List<FunctionNode> ChildList { get; }

        public bool Authrize { get; private set; }

        public FunctionNode(string functionName)
        {
            FunctionName = functionName;
            ChildList = new List<FunctionNode>();
        }

        public FunctionNode(string functionName, string absoluteUrl, string relativeUrl, bool authrize = false)
        {
            FunctionName = functionName;
            AbsoluteUrl = absoluteUrl;
            RelativeUrl = relativeUrl;
            ChildList = new List<FunctionNode>();
            Authrize = authrize;
        }

        public void AllAuthrized()
        {
            Authrize = true;
            ChildList.ForEach(x => x.AllAuthrized());
        }

        public void UnAuthrized()
        {
            Authrize = false;
        }

        public void CollectAuthrized()
        {
            ChildList.ForEach(x => x.CollectAuthrized());
            if (ChildList.Where(x => x.Authrize == true).Count() != ChildList.Count())
            {
                UnAuthrized();
            }
        }

        public static bool operator ==(FunctionNode me, object other) => me?.Equals(other) == true;

        public static bool operator !=(FunctionNode me, object other) => !me?.Equals(other) == true;
    }
}
