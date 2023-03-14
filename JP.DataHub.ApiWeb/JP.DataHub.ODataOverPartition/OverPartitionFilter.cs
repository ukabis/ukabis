using JP.DataHub.Aop;

namespace JP.DataHub.ODataOverPartition
{
    public class OverPartitionFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            param.IsOverPartition = true;
            return null;
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            return msg;
        }
    }
}
