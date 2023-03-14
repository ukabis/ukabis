using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData;
using Microsoft.Spatial;

namespace JP.DataHub.OData.SqlServerDb.ObsoleteODataToSqlTranslator
{
    /// <summary>
    /// Build QueryNode to string Representation 
    /// </summary>
    [Obsolete("互換性のために残している旧版です。")]
    internal class ODataNodeToStringBuilder : QueryNodeVisitor<string>
    {
        /// <summary>
        /// whether translating search options or others
        /// </summary>
        private bool searchFlag;
        /// <summary>
        /// whether translating an AnyClause or not
        /// </summary>
        private bool joinClause;
        /// <summary>s
        /// Gets the formatter to format the query
        /// </summary>
        private IQueryFormatter QueryFormatter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ODataNodeToStringBuilder"/> class
        /// </summary>
        /// <param name="queryFormatter">the query format class</param>
        public ODataNodeToStringBuilder(IQueryFormatter queryFormatter)
        {
            this.QueryFormatter = queryFormatter;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="ODataNodeToStringBuilder"/> class from being created
        /// </summary>
        private ODataNodeToStringBuilder()
        {
        }

        /// <summary>
        /// Translates a <see cref="AllNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(AllNode node)
        {
            var result = string.Concat(Constants.Delimiter, this.TranslateNode(node.Source, true), Constants.Delimiter, this.TranslateNode(node.Body));

            return result;
        }

        /// <summary>
        /// Translates a <see cref="AnyNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(AnyNode node)
        {
            //should return something like JOIN a in c.companies 
            //if (node.CurrentRangeVariable == null && node.Body.Kind == QueryNodeKind.Constant)
            //{
            //    return string.Concat(Constants.Delimiter, this.TranslateNode(node.Source, true), Constants.SymbolForwardSlash, Constants.KeywordAny, Constants.SymbolOpenParen, Constants.SymbolClosedParen, Constants.Delimiter);
            //}
            //else
            //{
            string source = TranslateNode(node.Source, true);
            string body = TranslateNode(node.Body);
            return string.Concat(Constants.Delimiter, source, Constants.Delimiter, Constants.OpenParen, body, Constants.ClosedParen);
            //}
        }

        /// <summary>
        /// Translates a <see cref="BinaryOperatorNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(BinaryOperatorNode node)
        {
            var leftNode = node.Left;
            while (leftNode != null && leftNode.Kind == QueryNodeKind.Convert)
            {
                leftNode = ((ConvertNode)leftNode).Source;
            }

            var rightNode = node.Right;
            while (rightNode != null && rightNode.Kind == QueryNodeKind.Convert)
            {
                rightNode = ((ConvertNode)rightNode).Source;
            }

            var left = this.TranslateNode(node.Left);
            if (leftNode.Kind == QueryNodeKind.BinaryOperator && TranslateBinaryOperatorPriority(((BinaryOperatorNode)leftNode).OperatorKind) < TranslateBinaryOperatorPriority(node.OperatorKind))
            {
                left = string.Concat(Constants.OpenParen, left, Constants.ClosedParen);
            }

            var right = this.TranslateNode(node.Right);
            if (rightNode.Kind == QueryNodeKind.BinaryOperator && TranslateBinaryOperatorPriority(((BinaryOperatorNode)rightNode).OperatorKind) < TranslateBinaryOperatorPriority(node.OperatorKind))
            {
                right = string.Concat(Constants.OpenParen, right, Constants.ClosedParen);
            }

            return string.Concat(left, Constants.Space, BinaryOperatorNodeToString(node.OperatorKind), Constants.Space, right);
        }

        /// <summary>
        /// Translates a <see cref="CollectionNavigationNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(CollectionNavigationNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.NavigationProperty.Name, node.NavigationSource);
        }

        /// <summary>
        /// Translates a <see cref="CollectionPropertyAccessNode"/> into a corresponding.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(CollectionPropertyAccessNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.Property.Name);
        }

        /// <summary>
        /// Translates a <see cref="CollectionPropertyAccessNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(ConstantNode node)
        {
            if (node.Value == null)
            {
                return ODataKeywords.Null;
            }

            if (node.TypeReference.IsEnum())
            {
                var specificNode = (ODataEnumValue)node.Value;

                return this.QueryFormatter.TranslateEnumValue(node.TypeReference, specificNode.Value);
            }
            else if (node.TypeReference.IsGuid())
            {
                return string.Format("'{0}'", node.Value);
            }
            // Translate Geography
            else if (node.TypeReference.IsGeography())
            {
                Func<GeographyLineString, string> createLineString = (GeographyLineString lineString) =>
                {
                    var coordinates = new StringBuilder();
                    foreach (var point in lineString.Points)
                    {
                        if (!point.IsEmpty)
                        {
                            if (coordinates.Length > 0) coordinates.Append(",");
                            coordinates.Append($"{point.Longitude} {point.Latitude}");
                        }
                    }

                    return $"({coordinates})";
                };

                // Translates Point
                if (node.TypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.GeographyPoint)
                {
                    var point = node.Value as GeographyPoint;
                    if (point?.IsEmpty == false)
                    {
                        return $"geography::STGeomFromText('POINT({point.Longitude},{point.Latitude})', {point.CoordinateSystem.EpsgId})";
                    }
                }
                // Translate Polygon
                else if (node.TypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.GeographyPolygon)
                {
                    var polygon = node.Value as GeographyPolygon;
                    if (polygon?.IsEmpty == false)
                    {
                        var lineStrings = new StringBuilder();
                        foreach (var lineString in polygon.Rings)
                        {
                            if (!lineString.IsEmpty)
                            {
                                if (lineStrings.Length > 0) lineStrings.Append(",");
                                lineStrings.Append(createLineString(lineString));
                            }
                        }

                        return $"geography::STGeomFromText('POLYGON({lineStrings})', {polygon.CoordinateSystem.EpsgId})";
                    }
                }
                // Translate LineString
                else if (node.TypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.GeographyLineString)
                {
                    var lineString = node.Value as GeographyLineString;
                    if (lineString?.IsEmpty == false)
                    {
                        return createLineString(lineString);
                    }
                }
            }

            return node.LiteralText;
        }

        /// <summary>
        /// Translates a <see cref="ConvertNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(ConvertNode node)
        {
            return this.TranslateNode(node.Source);
        }

        /// <summary>
        /// Translates a <see cref="CollectionResourceCastNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string of EntityCollectionCastNode.</returns>
        public override string Visit(CollectionResourceCastNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.ItemStructuredType.Definition.ToString());
        }

        /// <summary>
        /// Translates a <see cref="ResourceRangeVariableReferenceNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(ResourceRangeVariableReferenceNode node)
        {
            if (node.Name == "$it")
            {
                return string.Empty;
            }
            else
            {
                return node.Name;
            }
        }

        /// <summary>
        /// Translates a <see cref="NonResourceRangeVariableReferenceNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(NonResourceRangeVariableReferenceNode node)
        {
            return node.Name;
        }

        /// <summary>
        /// Translates a <see cref="SingleResourceCastNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(SingleResourceCastNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.TypeReference.Definition.ToString());
        }

        /// <summary>
        /// Translates a <see cref="SingleNavigationNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(SingleNavigationNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.NavigationProperty.Name, node.NavigationSource);
        }

        /// <summary>
        /// Translates a <see cref="SingleResourceFunctionCallNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(SingleResourceFunctionCallNode node)
        {
            string result = node.Name;
            if (node.Source != null)
            {
                result = this.TranslatePropertyAccess(node.Source, result);
            }

            return this.TranslateFunctionCall(result, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="SingleValueFunctionCallNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(SingleValueFunctionCallNode node)
        {
            string result = node.Name;
            if (node.Source != null)
            {
                result = this.TranslatePropertyAccess(node.Source, result);
            }

            return this.TranslateFunctionCall(result, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="CollectionFunctionCallNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string of CollectionFunctionCallNode.</returns>
        public override string Visit(CollectionFunctionCallNode node)
        {
            string result = node.Name;
            if (node.Source != null)
            {
                result = this.TranslatePropertyAccess(node.Source, result);
            }

            return this.TranslateFunctionCall(result, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="CollectionResourceFunctionCallNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string of EntityCollectionFunctionCallNode.</returns>
        public override string Visit(CollectionResourceFunctionCallNode node)
        {
            string result = node.Name;
            if (node.Source != null)
            {
                result = this.TranslatePropertyAccess(node.Source, result);
            }

            return this.TranslateFunctionCall(result, node.Parameters);
        }

        /// <summary>
        /// Translates a <see cref="SingleValueOpenPropertyAccessNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(SingleValueOpenPropertyAccessNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.Name);
        }

        /// <summary>
        /// Translates an <see cref="CollectionOpenPropertyAccessNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(CollectionOpenPropertyAccessNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.Name);
        }

        /// <summary>
        /// Translates a <see cref="SingleValuePropertyAccessNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(SingleValuePropertyAccessNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.Property.Name);
        }

        /// <summary>
        /// Translates a <see cref="ParameterAliasNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(ParameterAliasNode node)
        {
            return node.Alias;
        }

        /// <summary>
        /// Translates a <see cref="NamedFunctionParameterNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string of NamedFunctionParameterNode.</returns>
        public override string Visit(NamedFunctionParameterNode node)
        {
            return string.Concat(node.Name, Constants.Equal, this.TranslateNode(node.Value));
        }

        /// <summary>
        /// Translates a <see cref="NamedFunctionParameterNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string of SearchTermNode.</returns>
        public override string Visit(SearchTermNode node)
        {
            return node.Text;
        }

        /// <summary>
        /// Translates a <see cref="UnaryOperatorNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(UnaryOperatorNode node)
        {
            string result = null;
            if (node.OperatorKind == UnaryOperatorKind.Negate)
            {
                result = Constants.Negate;
            }

            // if current translated node is SearchNode, the UnaryOperator should return NOT, or return not
            if (node.OperatorKind == UnaryOperatorKind.Not)
            {
                if (this.searchFlag)
                {
                    result = ODataKeywords.SearchNot;
                }
                else
                {
                    result = ODataKeywords.Not;
                }
            }

            if (node.Operand.Kind == QueryNodeKind.Constant || node.Operand.Kind == QueryNodeKind.SearchTerm)
            {
                return string.Concat(result, ' ', this.TranslateNode(node.Operand));
            }
            else
            {
                return string.Concat(result, Constants.OpenParen, this.TranslateNode(node.Operand), Constants.ClosedParen);
            }
        }

        public override string Visit(InNode node)
        {
            var left = node.Left as SingleValuePropertyAccessNode;
            // 理由は不明だが、ODataの構文解析をすると、「$filter=id eq A」の「id」が「Id」になってしまう。（１文字目が大文字）
            // その対処のため、Nameを変更する
            if (left?.Property?.Name == "Id")
            {
                left = new SingleValuePropertyAccessNode(left.Source, new EdmStructuralProperty(left.Property.DeclaringType, "id", left.Property.Type));
            }
            var result = $"{TranslateNode(left)} in ({TranslateNode(node.Right)})";
            return result;
        }

        public override string Visit(CollectionConstantNode node)
        {
            string result = string.Join(Constants.Comma.ToString(), node.Collection.Select(x => "'" + TranslateNode(x) + "'").ToArray());
            return result;
        }

        /// <summary>Translates a <see cref="LevelsClause"/> into a string.</summary>
        /// <param name="levelsClause">The levels clause to translate.</param>
        /// <returns>The translated string.</returns>
        internal static string TranslateLevelsClause(LevelsClause levelsClause)
        {
            string levelsStr = levelsClause.IsMaxLevel
                ? ODataKeywords.Max
                : levelsClause.Level.ToString(CultureInfo.InvariantCulture);
            return levelsStr;
        }


        /// <summary>
        /// Main dispatching visit method for translating query-nodes into expressions.
        /// </summary>
        /// <param name="node">The node to visit/translate.</param>
        /// <param name="joinClause">true if join must be extracted</param>
        /// <returns>The LINQ string resulting from visiting the node.</returns>
        internal string TranslateNode(QueryNode node, bool joinClause = false)
        {
            if (joinClause)//starting
                this.joinClause = joinClause;

            return node.Accept(this);
        }

        /// <summary>Translates a <see cref="SearchClause"/> into a <see cref="SearchClause"/>.</summary>
        /// <param name="searchClause">The search clause to translate.</param>
        /// <returns>The translated string.</returns>
        internal string TranslateSearchClause(SearchClause searchClause)
        {
            this.searchFlag = true;
            var searchStr = this.TranslateNode(searchClause.Expression);
            this.searchFlag = false;
            return searchStr;
        }

        /// <summary>
        /// Helper for translating an access to a metadata-defined property or navigation.
        /// </summary>
        /// <param name="sourceNode">The source of the property access.</param>
        /// <param name="edmPropertyName">The structural or navigation property being accessed.</param>
        /// <param name="navigationSource">The navigation source of the result, required for navigations.</param>
        /// <returns>The translated string.</returns>
        private string TranslatePropertyAccess(QueryNode sourceNode, string edmPropertyName, IEdmNavigationSource navigationSource = null)
        {
            var source = this.TranslateNode(sourceNode);

            if (string.IsNullOrEmpty(source))
            {
                if (joinClause)
                    return this.QueryFormatter.TranslateJoinClause(edmPropertyName);
                else
                    return this.QueryFormatter.TranslateFieldName(edmPropertyName);
            }
            else
            {
                if (joinClause)
                    return this.QueryFormatter.TranslateJoinClause(source, edmPropertyName);
                else
                    return this.QueryFormatter.TranslateSource(source, edmPropertyName);
            }
        }

        /// <summary>
        /// Translates a function call into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="argumentNodes">The argument nodes.</param>
        /// <returns>
        /// The translated string.
        /// </returns>
        private string TranslateFunctionCall(string functionName, IEnumerable<QueryNode> argumentNodes)
        {
            string format = null;

            switch (functionName)
            {
                case ODataKeywords.Contains:
                    format = "'%' + {0} + '%'";
                    break;

                case ODataKeywords.StartsWith:
                    format = "{0} + '%'";
                    break;

                case ODataKeywords.EndsWith:
                    format = "'%' + {0}";
                    break;
            }

            if (!string.IsNullOrEmpty(format) && argumentNodes.Count() == 2)
                return $"{TranslateNode(argumentNodes.First())} {SqlKeywords.Like} {string.Format(format, TranslateNode(argumentNodes.Last()))}";

            if ((functionName == ODataKeywords.GeoDistance || functionName == ODataKeywords.GeoIntersects) && argumentNodes.Count() == 2)
                return string.Concat(TranslateNode(argumentNodes.First()), Constants.Dot, QueryFormatter.TranslateFunctionName(functionName),
                    Constants.OpenParen, TranslateNode(argumentNodes.Last()), Constants.ClosedParen);

            string result = string.Empty;
            foreach (QueryNode queryNode in argumentNodes)
            {
                result = string.Concat(result, string.IsNullOrEmpty(result) ? null : Constants.Comma.ToString(), this.TranslateNode(queryNode));
            }

            var translatedFunctionCall = string.Concat(QueryFormatter.TranslateFunctionName(functionName), Constants.OpenParen, result, Constants.ClosedParen);
            return translatedFunctionCall;
        }

        /// <summary>
        /// Build BinaryOperatorNode to uri 
        /// </summary>
        /// <param name="operatorKind">the kind of the BinaryOperatorNode</param>
        /// <returns>string format of the operator</returns>
        private static string BinaryOperatorNodeToString(BinaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case BinaryOperatorKind.Or:
                    return SqlKeywords.Or;
                case BinaryOperatorKind.And:
                    return SqlKeywords.And;
                case BinaryOperatorKind.Equal:
                    return SqlKeywords.Equal;
                case BinaryOperatorKind.NotEqual:
                    return SqlKeywords.NotEqual;
                case BinaryOperatorKind.GreaterThan:
                    return SqlKeywords.GreaterThan;
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return SqlKeywords.GreaterThanOrEqual;
                case BinaryOperatorKind.LessThan:
                    return SqlKeywords.LessThan;
                case BinaryOperatorKind.LessThanOrEqual:
                    return SqlKeywords.LessThanOrEqual;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Get the priority of BinaryOperatorNode
        /// This priority table is from <c>http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part2-url-conventions.html</c> (5.1.1.9 Operator Precedence )
        /// </summary>
        /// <param name="operatorKind">binary operator </param>
        /// <returns>the priority value of the binary operator</returns>
        private static int TranslateBinaryOperatorPriority(BinaryOperatorKind operatorKind)
        {
            switch (operatorKind)
            {
                case BinaryOperatorKind.Or:
                    return 1;
                case BinaryOperatorKind.And:
                    return 2;
                case BinaryOperatorKind.Equal:
                case BinaryOperatorKind.NotEqual:
                case BinaryOperatorKind.GreaterThan:
                case BinaryOperatorKind.GreaterThanOrEqual:
                case BinaryOperatorKind.LessThan:
                case BinaryOperatorKind.LessThanOrEqual:
                    return 3;
                case BinaryOperatorKind.Add:
                case BinaryOperatorKind.Subtract:
                    return 4;
                case BinaryOperatorKind.Divide:
                case BinaryOperatorKind.Multiply:
                case BinaryOperatorKind.Modulo:
                    return 5;
                case BinaryOperatorKind.Has:
                    return 6;
                default:
                    return -1;
            }
        }
    }
}
