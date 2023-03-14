using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.IT.Api.Com.WebApi.Attributes
{
    public class GenerateApiControl : Attribute
    {
        public enum GenerateControl
        {
            NoTransparent,
            NoDefault,
        }

        private List<GenerateControl> generateControl = null;

        public GenerateApiControl(params GenerateControl[] generateControl)
        {
            this.generateControl = new List<GenerateControl>(generateControl);
        }
    }
}
