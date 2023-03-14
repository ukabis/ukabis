using JP.DataHub.Aop;

namespace JP.DataHub.ODataOverPartition
{
    /// <summary>
    /// 領域越えODataDeleteのAOP
    /// AOP対象のAPIを領域越えのODataDeleteに差し替える
    /// ※このAOPが存在する理由は、ODataは透過APIであり、その透過APIの設定は画面上から変更できない（セキュリティ観点）
    /// よってAOPにより領域超えを実現するためにAOPで行っている（AOPでやる＝運営側が理解して設定しているから）
    /// </summary>
    public class ODataDeleteOverPartitionFilter : AbstractApiFilter
    {
        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            param.IsOverPartition = true;
            param.Action = "odd";
            return null;
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            return msg;
        }
    }

}