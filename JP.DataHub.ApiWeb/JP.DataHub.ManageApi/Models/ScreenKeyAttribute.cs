using System;

namespace JP.DataHub.ManageApi.Models
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ScreenKeyAttribute : Attribute
    {
        public string ScreenTitle { get; set; }

        public string ScreenKeyString { get; set; }

        public ScreenKeyAttribute(string screenTitle, string screenKey)
        {
            this.ScreenTitle = screenTitle;
            this.ScreenKeyString = screenKey;
        }
    }
}
