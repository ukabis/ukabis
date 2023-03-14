using System;

namespace JP.DataHub.ManageApi.Models
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ScreenIdNameAttribute : Attribute
    {
        public string Name { get; set; }

        public ScreenIdNameAttribute(string name = null)
        {
            Name = name;
        }
    }
}
