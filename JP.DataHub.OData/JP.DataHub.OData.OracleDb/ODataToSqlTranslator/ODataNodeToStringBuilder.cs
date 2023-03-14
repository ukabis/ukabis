using GeoJSON.Net.Geometry;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData;
using Microsoft.Spatial;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JP.DataHub.OData.OracleDb.ODataToSqlTranslator
{
    /// <summary>
    /// Build QueryNode to string Representation 
    /// </summary>
    internal class ODataNodeToStringBuilder : QueryNodeVisitor<string>
    {
        /// <summary>s
        /// Gets the formatter to format the query
        /// </summary>
        public IQueryFormatter QueryFormatter { get; set; }

        /// <summary>
        /// whether translating search options or others
        /// </summary>
        private bool searchFlag;
        /// <summary>
        /// whether translating an AnyClause or not
        /// </summary>
        private Dictionary<string, object> Parameters { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ODataNodeToStringBuilder"/> class
        /// </summary>
        /// <param name="queryFormatter">the query format class</param>
        public ODataNodeToStringBuilder(IQueryFormatter queryFormatter, Dictionary<string, object> paramters)
        {
            this.QueryFormatter = queryFormatter;
            this.Parameters = paramters;
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
            var result = string.Concat(Constants.Delimiter, this.TranslateNode(node.Source), Constants.Delimiter, this.TranslateNode(node.Body));

            return result;
        }

        /// <summary>
        /// Translates a <see cref="AnyNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(AnyNode node)
        {
            throw new NotSupportedException("any");
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

            var isLogicalOperation = IsLogicalOperation(node.OperatorKind);
            var left = this.TranslateNode(leftNode, isLogicalOperation && IsBooleanPropertyNode(leftNode));
            if (leftNode.Kind == QueryNodeKind.BinaryOperator && TranslateBinaryOperatorPriority(((BinaryOperatorNode)leftNode).OperatorKind) < TranslateBinaryOperatorPriority(node.OperatorKind))
            {
                left = string.Concat(Constants.SymbolOpenParen, left, Constants.SymbolClosedParen);
            }
            var right = this.TranslateNode(rightNode, isLogicalOperation && IsBooleanPropertyNode(rightNode));
            if (rightNode.Kind == QueryNodeKind.BinaryOperator && TranslateBinaryOperatorPriority(((BinaryOperatorNode)rightNode).OperatorKind) < TranslateBinaryOperatorPriority(node.OperatorKind))
            {
                right = string.Concat(Constants.SymbolOpenParen, right, Constants.SymbolClosedParen);
            }

            if (leftNode.Kind == QueryNodeKind.SingleValueOpenPropertyAccess &&
                rightNode.Kind == QueryNodeKind.Constant)
            {
                ValidateAndFormat((SingleValueOpenPropertyAccessNode)leftNode, right);
            }
            else if (rightNode.Kind == QueryNodeKind.SingleValueOpenPropertyAccess &&
                     leftNode.Kind == QueryNodeKind.Constant)
            {
                ValidateAndFormat((SingleValueOpenPropertyAccessNode)rightNode, left);
            }

            if (right == Constants.KeywordNull)
            {
                return string.Concat(left, Constants.SymbolSpace, Constants.SQLIsSymbol, Constants.SymbolSpace, Constants.KeywordNull);
            }
            else if (left == Constants.KeywordNull)
            {
                return string.Concat(right, Constants.SymbolSpace, Constants.SQLIsSymbol, Constants.SymbolSpace, Constants.KeywordNull);
            }
            else
            {
                return string.Concat(left, Constants.SymbolSpace, BinaryOperatorNodeToString(node.OperatorKind), Constants.SymbolSpace, right);
            }
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
                return Constants.KeywordNull;
            }

            // Translate Geography
            if (node.TypeReference.IsGeography())
            {
                Func<GeographyLineString, LineString> createLineString = (GeographyLineString lineString) =>
                {
                    var coordinates = new List<IPosition>();
                    foreach (var point in lineString.Points)
                    {
                        if (!point.IsEmpty)
                        {
                            var position = new Position(point.Latitude, point.Longitude, point.Z);
                            coordinates.Add(position);
                        }
                    }

                    return new LineString(coordinates);
                };

                // Translates Point
                if (node.TypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.GeographyPoint)
                {
                    var point = node.Value as GeographyPoint;
                    if (point?.IsEmpty == false)
                    {
                        var position = new Position(point.Latitude, point.Longitude, point.Z);
                        return JsonConvert.SerializeObject(new Point(position));
                    }
                }
                // Translate Polygon
                else if (node.TypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.GeographyPolygon)
                {
                    var polygon = node.Value as GeographyPolygon;
                    if (polygon?.IsEmpty == false)
                    {
                        var lineStrings = new List<LineString>();
                        foreach (var lineString in polygon.Rings)
                        {
                            if (!lineString.IsEmpty)
                            {
                                lineStrings.Add(createLineString(lineString));
                            }
                        }

                        return JsonConvert.SerializeObject(new Polygon(lineStrings));
                    }
                }
                // Translate LineString
                else if (node.TypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.GeographyLineString)
                {
                    var lineString = node.Value as GeographyLineString;
                    if (lineString?.IsEmpty == false)
                    {
                        return JsonConvert.SerializeObject(createLineString(lineString));
                    }
                }
            }

            // 文字列、数値、Boolean、GUID、ENUM
            return LiteralToParameter(node);
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

        ///// <summary>
        ///// Visit an CollectionResourceCastNode
        ///// </summary>
        ///// <param name="node">the node to visit</param>
        ///// <returns>The translated string of CollectionPropertyCastNode</returns>
        //public override string Visit(CollectionPropertyCastNode node)
        //{
        //    return this.TranslatePropertyAccess(node.Source, node.CollectionType.Definition.ToString());
        //}

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
            return string.Concat(node.Name, Constants.SymbolEqual, this.TranslateNode(node.Value));
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
                result = Constants.SymbolNegate;
            }

            // if current translated node is SearchNode, the UnaryOperator should return NOT, or return not
            if (node.OperatorKind == UnaryOperatorKind.Not)
            {
                if (this.searchFlag)
                {
                    result = Constants.SearchKeywordNot;
                }
                else
                {
                    result = Constants.KeywordNot;
                }
            }

            if (node.Operand.Kind == QueryNodeKind.Constant || node.Operand.Kind == QueryNodeKind.SearchTerm)
            {
                return string.Concat(result, ' ', this.TranslateNode(node.Operand));
            }
            else
            {
                return string.Concat(result, Constants.SymbolOpenParen, this.TranslateNode(node.Operand, true), Constants.SymbolClosedParen);
            }
        }

        public override string Visit(InNode node)
        {
            var left = node.Left as SingleValueOpenPropertyAccessNode;
            var result = $"{TranslateNode(left)} in ({TranslateNode(node.Right)})";
            return result;
        }

        public override string Visit(CollectionConstantNode node)
        {
            string result = string.Join(Constants.SymbolComma.ToString(), node.Collection.Select(x => TranslateNode(x)).ToArray());
            return result;
        }

        /// <summary>Translates a <see cref="LevelsClause"/> into a string.</summary>
        /// <param name="levelsClause">The levels clause to translate.</param>
        /// <returns>The translated string.</returns>
        internal static string TranslateLevelsClause(LevelsClause levelsClause)
        {
            string levelsStr = levelsClause.IsMaxLevel
                ? Constants.KeywordMax
                : levelsClause.Level.ToString(CultureInfo.InvariantCulture);
            return levelsStr;
        }


        /// <summary>
        /// Main dispatching visit method for translating query-nodes into expressions.
        /// </summary>
        /// <param name="node">The node to visit/translate.</param>
        /// <param name="convertIfBooleanProperty">Boolean型のプロパティであればbit型から変換して出力する</param>
        /// <returns>The LINQ string resulting from visiting the node.</returns>
        internal string TranslateNode(QueryNode node, bool convertIfBooleanProperty = false)
        {
            if (convertIfBooleanProperty && IsBooleanPropertyNode(node))
            {
                // bit型がSQL上でboolとして評価されるよう変換："(isnull({propertyName},0)=1)"
                return string.Concat(Constants.SymbolOpenParen, Constants.KeywordIsNull, Constants.SymbolOpenParen, node.Accept(this), Constants.SymbolComma, 0, Constants.SymbolClosedParen, Constants.SymbolEqual, 1, Constants.SymbolClosedParen);
            }
            else
            {
                return node.Accept(this);
            }
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
                return this.QueryFormatter.TranslateFieldName(edmPropertyName);
            }
            else
            {
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
            // 特殊な変換処理
            List<QueryNode> list = argumentNodes.ToList();
            switch (functionName)
            {
                case Constants.KeywordSubstring:
                    // OracleのSUBSTRは1スタートのため開始位置を修正
                    var functionParams = new List<string>();
                    for (var i = 0; i < list.Count; i++)
                    {
                        var value = this.TranslateNode(list[i]);
                        functionParams.Add(value);
                    }
                    return string.Concat(QueryFormatter.TranslateFunctionName(functionName), Constants.SymbolOpenParen, string.Join(Constants.SymbolComma.ToString(), functionParams), Constants.SymbolClosedParen);
                case Constants.KeywordContains:
                    return $"{TranslateNode(list[0])} LIKE '%' || {TranslateNode(list[1])} || '%'";
                case Constants.KeywordStartwith:
                    return $"{TranslateNode(list[0])} LIKE {TranslateNode(list[1])} || '%'";
                case Constants.KeywordEndswith:
                    return $"{TranslateNode(list[0])} LIKE '%' || {TranslateNode(list[1])}";
                case Constants.KeywordIndexOf:
                    // 他のデータソースに合わせて0基準に修正
                    var x = string.Concat(QueryFormatter.TranslateFunctionName(functionName), Constants.SymbolOpenParen, TranslateNode(list[0]), Constants.SymbolComma, TranslateNode(list[1]), Constants.SymbolClosedParen);
                    return $"({x} - 1)";
                case Constants.KeywordRound:
                    return string.Concat(QueryFormatter.TranslateFunctionName(functionName), Constants.SymbolOpenParen, TranslateNode(list[0]), Constants.SymbolComma.ToString(), 0, Constants.SymbolClosedParen);
                case Constants.KeywordGeoIntersects:
                    throw new NotSupportedException(Constants.KeywordGeoIntersects);
                case Constants.KeywordGeoDistance:
                    throw new NotSupportedException(Constants.KeywordGeoDistance);
            }

            // 共通の変換処理
            string result = string.Empty;
            foreach (QueryNode queryNode in list)
            {
                result = string.Concat(result, string.IsNullOrEmpty(result) ? null : Constants.SymbolComma.ToString(), this.TranslateNode(queryNode));
            }
            return string.Concat(QueryFormatter.TranslateFunctionName(functionName), Constants.SymbolOpenParen, result, Constants.SymbolClosedParen);
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
                    return Constants.SQLOrSymbol;
                case BinaryOperatorKind.And:
                    return Constants.SQLAndSymbol;
                case BinaryOperatorKind.Equal:
                    return Constants.SQLEqualSymbol;
                case BinaryOperatorKind.NotEqual:
                    return Constants.SQLNotEqualSymbol;
                case BinaryOperatorKind.GreaterThan:
                    return Constants.SQLGreaterThanSymbol;
                case BinaryOperatorKind.GreaterThanOrEqual:
                    return Constants.SQLGreaterThanOrEqualSymbol;
                case BinaryOperatorKind.LessThan:
                    return Constants.SQLLessThanSymbol;
                case BinaryOperatorKind.LessThanOrEqual:
                    return Constants.SQLLessThanOrEqualSymbol;
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


        /// <summary>
        /// リテラルノードをパラメータに変換する。
        /// </summary>
        private string LiteralToParameter(ConstantNode node)
        {
            object parameterValue;
            if (node.TypeReference.IsEnum())
            {
                var specificNode = (ODataEnumValue)node.Value;
                var enumValue = this.QueryFormatter.TranslateEnumValue(node.TypeReference, specificNode.Value);
                if (enumValue.StartsWith("'") && enumValue.EndsWith("'"))
                {
                    parameterValue = enumValue.Substring(1, enumValue.Length - 2);
                }
                else
                {
                    parameterValue = enumValue;
                }
            }
            else if (node.TypeReference.IsBoolean())
            {
                parameterValue = (bool)node.Value;
            }
            else if (node.TypeReference.IsGuid())
            {
                parameterValue = node.Value.ToString().ToUpper();
            }
            else if (decimal.TryParse(node.LiteralText, out var number))
            {
                parameterValue = number;
            }
            else if (node.LiteralText.StartsWith("'") && node.LiteralText.EndsWith("'"))
            {
                // 16進数が混入したクエリをデコード
                var value = QueryFormatter.IsFilterValueUnescapeEnabled ? Regex.Unescape(node.LiteralText) : node.LiteralText;

                // 文字列リテラルの場合はエスケープを解除
                parameterValue = value.Substring(1, value.Length - 2).Replace("''", "'");
            }
            else
            {
                // 16進数が混入したクエリをデコード
                parameterValue = QueryFormatter.IsFilterValueUnescapeEnabled ? Regex.Unescape(node.LiteralText) : node.LiteralText;
            }

            var parameterName = $":o_param{Parameters.Count + 1}";
            Parameters.Add(parameterName, parameterValue);

            return parameterName;
        }


        /// <summary>
        /// パラメータの検証とフォーマット
        /// </summary>
        private void ValidateAndFormat(SingleValueOpenPropertyAccessNode node, string param)
        {
            if (!Parameters.ContainsKey(param))
            {
                return;
            }

            Parameters[param] = QueryFormatter.ValidateAndFormatValue(node.Name, Parameters[param]);
        }

        /// <summary>
        /// 論理演算子かどうか
        /// </summary>
        private bool IsLogicalOperation(BinaryOperatorKind kind)
        {
            return kind == BinaryOperatorKind.And || kind == BinaryOperatorKind.Or;
        }

        /// <summary>
        /// Booleanプロパティのノードかどうか
        /// </summary>
        private bool IsBooleanPropertyNode(QueryNode node)
        {
            return node is SingleValueOpenPropertyAccessNode property && QueryFormatter.IsBooleanProperty(property.Name);
        }
    }
}
