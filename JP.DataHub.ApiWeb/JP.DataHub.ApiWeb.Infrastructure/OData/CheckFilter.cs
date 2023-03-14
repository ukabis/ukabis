using Microsoft.OData.UriParser;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Infrastructure.OData
{
    public static class CheckFilter
    {
        private static List<string> s_invalidColumns = new List<string>();


        static CheckFilter()
        {
            s_invalidColumns = UnityCore.Resolve<string[]>("InvalidODataColums")?.ToList() ?? new List<string>();
        }


        public static bool CheckODataFilter(this SingleValueNode singleValueNode)
        {
            if (singleValueNode.Kind == QueryNodeKind.BinaryOperator)
            {
                var tempNode = singleValueNode as BinaryOperatorNode;
                var rightResult = CheckODataFilter(tempNode.Right);
                var leftResult = CheckODataFilter(tempNode.Left);
                if (rightResult || leftResult)
                {
                    return true;
                }

            }
            if (singleValueNode.Kind == QueryNodeKind.SingleValueFunctionCall)
            {
                foreach (var param in ((SingleValueFunctionCallNode)singleValueNode).Parameters)
                {
                    if (CheckODataFilter((SingleValueNode)param))
                    {
                        return true;
                    }
                }
            }

            if (singleValueNode.Kind == QueryNodeKind.SingleValueOpenPropertyAccess && s_invalidColumns.Contains(((SingleValueOpenPropertyAccessNode)singleValueNode).Name))
            {
                return true;
            }

            if (singleValueNode.Kind == QueryNodeKind.Convert)
            {
                switch (((ConvertNode)singleValueNode).Source)
                {
                    case SingleValueFunctionCallNode singleValueFunctionCallNode:
                        foreach (var param in singleValueFunctionCallNode.Parameters)
                        {
                            if (CheckODataFilter((SingleValueNode)param))
                            {
                                return true;
                            }
                        }
                        break;
                    case SingleValueOpenPropertyAccessNode singleValueOpenPropertyAccessNode when s_invalidColumns.Contains(singleValueOpenPropertyAccessNode.Name):
                        return true;
                    case BinaryOperatorNode binaryOperatorNode:
                        return CheckODataFilter(binaryOperatorNode);

                }

            }
            return false;
        }
    }
}
