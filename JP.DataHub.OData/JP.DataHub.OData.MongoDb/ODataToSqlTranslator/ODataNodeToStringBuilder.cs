using System.Globalization;
using GeoJSON.Net.Geometry;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.Spatial;
using Newtonsoft.Json;
using JP.DataHub.OData.Interface.Exceptions;

namespace JP.DataHub.OData.MongoDb.ODataToSqlTranslator
{
    /// <summary>
    /// Build QueryNode to string Representation
    /// </summary>
    internal class ODataNodeToStringBuilder : QueryNodeVisitor<string>
    {
        /// <summary>
        /// whether translating search options or others
        /// </summary>
        private bool searchFlag;
        /// <summary>
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
            return $"{{ '{TranslateNode(node.Source)}' : {{ '$elemMatch' : {TranslateNode(node.Body)} }} }}";
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

            // 文字列関数の場合左辺と右辺のノードと演算子を同時に渡す
            if (leftNode?.Kind == QueryNodeKind.SingleValueFunctionCall || rightNode?.Kind == QueryNodeKind.SingleValueFunctionCall)
            {
                // 引数に地理空間データが含まれる場合地理空間関数とみなす
                if ((leftNode is SingleValueFunctionCallNode leftTmp && leftTmp.Parameters.Any(x => x is ConstantNode leftConstTmp && leftConstTmp.TypeReference.IsGeography())) ||
                    (rightNode is SingleValueFunctionCallNode rightTmp && rightTmp.Parameters.Any(x => x is ConstantNode rightConstTmp && rightConstTmp.TypeReference.IsGeography())))
                {
                    var tmp = (leftNode is SingleValueFunctionCallNode ? leftNode : rightNode) as SingleValueFunctionCallNode; // 関数名を持つノード
                    var another = leftNode is SingleValueFunctionCallNode ? rightNode : leftNode; // 比較対象の定数（数値）ノード
                    return another is ConstantNode && another.TypeReference.IsDouble() //  ※数値以外の場合はエラー
                        ? TranslateFunctionCall(tmp.Name, new List<QueryNode>(tmp.Parameters) { another }, BinaryOperatorNodeToString(node.OperatorKind))
                        : throw new ODataNotConvertibleToQueryException($"比較対象が不正です。'{tmp.Name}'の比較対象は定数（数値）である必要があります。");
                }

                // 戻り値がEdm.Boolean型以外の文字列関数は$expr演算子でネストする
                var leftNodeStr = leftNode is SingleValueOpenPropertyAccessNode ? ConvertToExpression(leftNode) : TranslateNode(leftNode);
                var rightNodeStr = rightNode is SingleValueOpenPropertyAccessNode ? ConvertToExpression(rightNode) : TranslateNode(rightNode);
                var translated = $"{{ '{BinaryOperatorNodeToString(node.OperatorKind)}' : [ {leftNodeStr} , {rightNodeStr} ] }}";
                return leftNode.TypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.Boolean || rightNode.TypeReference.PrimitiveKind() == EdmPrimitiveTypeKind.Boolean
                    ? translated
                    : $"{{ '$expr' : {translated} }}";
            }

            var left = this.TranslateNode(node.Left);
            if (leftNode.Kind == QueryNodeKind.BinaryOperator && TranslateBinaryOperatorPriority(((BinaryOperatorNode)leftNode).OperatorKind) < TranslateBinaryOperatorPriority(node.OperatorKind))
            {
                left = string.Concat(Constants.SymbolOpenParen, left, Constants.SymbolClosedParen);
            }

            var right = this.TranslateNode(node.Right);
            if (rightNode.Kind == QueryNodeKind.BinaryOperator && TranslateBinaryOperatorPriority(((BinaryOperatorNode)rightNode).OperatorKind) < TranslateBinaryOperatorPriority(node.OperatorKind))
            {
                right = string.Concat(Constants.SymbolOpenParen, right, Constants.SymbolClosedParen);
            }

            string result;
            if (node.OperatorKind == BinaryOperatorKind.Equal)
            {
                result = string.Concat($" {{ '{left}' : {right} }} ");
            }
            else if (node.OperatorKind >= BinaryOperatorKind.NotEqual && node.OperatorKind <= BinaryOperatorKind.LessThanOrEqual)
            {
                result = string.Concat($" {{ '{left}' : {{ '{BinaryOperatorNodeToString(node.OperatorKind)}' : {right} }} }} ");
            }
            else
            {
                result = $"{{ '{BinaryOperatorNodeToString(node.OperatorKind)}' : [ {left}, {right} ] }}";
                //result = string.Concat(left, Constants.SymbolSpace, BinaryOperatorNodeToString(node.OperatorKind), Constants.SymbolSpace, right);
            }
            return result;
        }

        /// <summary>
        /// Translates a <see cref="CollectionNavigationNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(CollectionNavigationNode node)
        {
            return this.TranslatePropertyAccess(node.Source, node.NavigationProperty.Name);
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

        public object VisitValue(ConstantNode node)
        {
            return node.Value;
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

            if (node.TypeReference.IsEnum())
            {
                var specificNode = (ODataEnumValue)node.Value;

                return this.QueryFormatter.TranslateEnumValue(node.TypeReference, specificNode.Value);
            }
            else if (node.TypeReference.IsGuid())
            {
                return string.Format("'{0}'", node.Value);
            }
            // 文字列リテラル
            else if (node.LiteralText.StartsWith("'") && node.LiteralText.EndsWith("'"))
            {
                // エスケープをOData形式からMongoDB形式に変換
                // Delimiter('|')はJOINの中間構文として使用しているため一時的に'||'にエスケープ
                return $"'{node.LiteralText.Substring(1, node.LiteralText.Length - 2).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("''", "\\'").Replace(Constants.Delimiter, Constants.Delimiter + Constants.Delimiter)}'";
            }
            // Translate Geography
            else if (node.TypeReference.IsGeography())
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
            return this.TranslatePropertyAccess(node.Source, node.NavigationProperty.Name);
        }

        //public override string Visit(SingleValueCastNode nodeIn)
        //{
        //    return base.Visit(nodeIn);
        //}
        //public override string Visit(SingleComplexNode nodeIn)
        //{
        //    return base.Visit(nodeIn);
        //}
        //public override string Visit(CountNode nodeIn)
        //{
        //    return base.Visit(nodeIn);
        //}

        //public override string Visit(CollectionComplexNode node)
        //{
        //    return base.Visit(node);
        //}

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
            return TranslatePropertyAccess(node.Source, node.Name);
        }

        /// <summary>
        /// Translates an <see cref="CollectionOpenPropertyAccessNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(CollectionOpenPropertyAccessNode node)
        {
            return TranslatePropertyAccess(node.Source, node.Name);
        }

        /// <summary>
        /// Translates a <see cref="SingleValuePropertyAccessNode"/> into a corresponding <see cref="string"/>.
        /// </summary>
        /// <param name="node">The node to translate.</param>
        /// <returns>The translated string.</returns>
        public override string Visit(SingleValuePropertyAccessNode node)
        {
            return TranslatePropertyAccess(node.Source, node.Property.Name);
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
                return string.Concat(result, Constants.SymbolOpenParen, this.TranslateNode(node.Operand), Constants.SymbolClosedParen);
            }
        }

        public override string Visit(InNode node)
        {
            var left = node.Left as SingleValueOpenPropertyAccessNode;
            var result = $"{{ '{TranslateNode(left)}' : {{ '$in' : [{TranslateNode(node.Right)}] }} }}";
            return result;
        }

        public override string Visit(CollectionConstantNode node)
        {
            string result = string.Join(Constants.SymbolComma.ToString(), node.Collection.Select(x => "'" + TranslateNode(x) + "'").ToArray());
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


        internal object ValueNode(QueryNode node) => (node is ConstantNode) ? (node as ConstantNode).Value : null;


        /// <summary>
        /// Main dispatching visit method for translating query-nodes into expressions.
        /// </summary>
        /// <param name="node">The node to visit/translate.</param>
        /// <returns>The LINQ string resulting from visiting the node.</returns>
        internal string TranslateNode(QueryNode node)
        {
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

        ///// <summary>
        ///// Add dictionary to url and each alias value will be URL encoded.
        ///// </summary>
        ///// <param name="dictionary">key value pair dictionary</param>
        ///// <returns>The url query string of dictionary's key value pairs (URL encoded)</returns>
        //internal string TranslateParameterAliasNodes(IDictionary<string, SingleValueNode> dictionary)
        //{
        //    string result = null;
        //    if (dictionary != null)
        //    {
        //        foreach (KeyValuePair<string, SingleValueNode> keyValuePair in dictionary)
        //        {
        //            if (keyValuePair.Value != null)
        //            {
        //                var tmp = this.TranslateNode(keyValuePair.Value);
        //                result = string.IsNullOrEmpty(tmp) ? result : string.Concat(result, string.IsNullOrEmpty(result) ? null : Constants.RequestParamsAggregator.ToString(), keyValuePair.Key, Constants.SymbolEqual, Uri.EscapeDataString(tmp));
        //            }
        //        }
        //    }

        //    return result;
        //}

        /// <summary>
        /// Helper for translating an access to a metadata-defined property or navigation.
        /// </summary>
        /// <param name="sourceNode">The source of the property access.</param>
        /// <param name="edmPropertyName">The structural or navigation property being accessed.</param>
        /// <param name="navigationSource">The navigation source of the result, required for navigations.</param>
        /// <returns>The translated string.</returns>
        private string TranslatePropertyAccess(QueryNode sourceNode, string edmPropertyName)
        {
            var source = this.TranslateNode(sourceNode);

            if (string.IsNullOrEmpty(source) || !(sourceNode is SingleValueOpenPropertyAccessNode))
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
        /// <param name="operatorString"></param>
        /// <returns>
        /// The translated string.
        /// </returns>
        private string TranslateFunctionCall(string functionName, IEnumerable<QueryNode> argumentNodes, string operatorString = null)
        {
            List<QueryNode> list = new List<QueryNode>(argumentNodes);
            switch (functionName)
            {
                case Constants.KeywordContains:
                    list = SortRegexExpressions(functionName, list);
                    return $"{{ {TranslateNode(list[0])} : /{ValueNode(list[1])}/ }}";
                case Constants.KeywordStartwith:
                    list = SortRegexExpressions(functionName, list);
                    return $"{{ {TranslateNode(list[0])} : /^{ValueNode(list[1])}/ }}";
                case Constants.KeywordEndswith:
                    list = SortRegexExpressions(functionName, list);
                    return $"{{ {TranslateNode(list[0])} : /{ValueNode(list[1])}$/ }}";
                case Constants.KeywordTrim:
                    // input: $trimの第一引数フィールド名
                    return $"{{ '$trim' : {{ input : {ConvertToExpressions(list)} }} }}";
                case Constants.KeywordLength:
                    // $ifNull: 該当のカラムが未定義の場合空を返す
                    return $"{{ '$strLenCP' : {{ '$ifNull': [ {ConvertToExpressions(list)}, '' ] }} }}";
                case Constants.KeywordIndexOf:
                    return $"{{ '$indexOfCP' : [ {ConvertToExpressions(list)} ] }}";
                case Constants.KeywordSubstring:
                    if (list.Count == 2)
                    {
                        // 第三引数なしの場合は自動補完する
                        return $"{{ '$substrCP' : [ {ConvertToExpressions(list)} , {int.MaxValue} ] }}";
                    }
                    else
                    {
                        return $"{{ '$substrCP' : [ {ConvertToExpressions(list)} ] }}";
                    }
                case Constants.KeywordConcat:
                    return $"{{ '$concat' : [ {ConvertToExpressions(list)} ] }}";
                case Constants.KeywordToLower:
                    return $"{{ '$toLower' : {ConvertToExpressions(list)} }}";
                case Constants.KeywordToUpper:
                    return $"{{ '$toUpper' : {ConvertToExpressions(list)} }}";
                case Constants.KeywordRound:
                    return $"{{ '$round' : {ConvertToExpressions(list)} }}";
                case Constants.KeywordFloor:
                    return $"{{ '$floor' : {ConvertToExpressions(list)} }}";
                case Constants.KeywordCeiling:
                    return $"{{ '$ceil' : {ConvertToExpressions(list)} }}";
                case Constants.KeywordGeoDistance:
                    list = SortGeoExpressions(functionName, list);
                    return $"{{ {TranslateNode(list[0])} : {{ '$near' : {{ '$geometry' : {TranslateNode(list[1])} {ConvertToMaxMinDistance(operatorString, ValueNode(list[2]))} }} }} }}";
                case Constants.KeywordGeoIntersects:
                    list = SortGeoExpressions(functionName, list);
                    return $"{{ {TranslateNode(list[0])} : {{ '$geoIntersects' : {{ '$geometry' : {TranslateNode(list[1])} }} }} }}";
                default:
                    return "{ }";
            }
        }

        /// <summary>
        /// 引数リストを文字列に変換する
        /// </summary>
        /// <param name="argumentList"></param>
        /// <returns></returns>
        private string ConvertToExpressions(List<QueryNode> argumentList)
        {
            return string.Join(" , ", argumentList.Select(x => ConvertToExpression(x)));
        }

        /// <summary>
        /// 引数を文字列に変換する
        /// </summary>
        /// <param name="argumentList"></param>
        /// <returns></returns>
        private string ConvertToExpression(QueryNode argument)
        {
            // 文字列関数 ※再帰的に処理
            if (argument is SingleValueFunctionCallNode ||
                (argument is ConvertNode node && node.Source is SingleValueFunctionCallNode))
            {
                return TranslateNode(argument);
            }

            var value = ValueNode(argument);
            if (value == null)
            {
                // 列名
                return $"'${TranslateNode(argument)}'";
            }
            else
            {
                // 定数
                return TranslateNode(argument);
            }
        }

        /// <summary>
        /// 正規表現を用いる関数の引数を並び替える（左辺:列名 右辺:定数）
        /// </summary>
        /// <param name="argumentList"></param>
        /// <returns></returns>
        private List<QueryNode> SortRegexExpressions(string functionName, List<QueryNode> argumentList)
        {
            return argumentList.Any(x => x is SingleValuePropertyAccessNode || x is ConvertNode) && argumentList.Any(x => x is ConstantNode)
                ? argumentList.OrderByDescending(x => x is SingleValuePropertyAccessNode || x is ConvertNode).ToList()
                : throw new ODataNotConvertibleToQueryException($"不正な引数が指定されました。'{functionName}'の引数は列名と定数（文字列）の組み合わせである必要があります。");
        }

        /// <summary>
        /// 地理空間関数の引数を並び替える（左辺:列名 右辺:地理空間データ）
        /// </summary>
        /// <param name="argumentList"></param>
        /// <returns></returns>
        private List<QueryNode> SortGeoExpressions(string functionName, List<QueryNode> argumentList)
        {
            return argumentList.Any(x => x is ConvertNode) && argumentList.Any(x => x is ConstantNode)
                ? argumentList.OrderByDescending(x => x is ConvertNode).ToList()
                : throw new ODataNotConvertibleToQueryException($"不正な引数が指定されました。'{functionName}'の引数は列名と地理空間データの組み合わせである必要があります。");
        }

        /// <summary>
        /// 下限距離・上限距離プロパティを作成する
        /// </summary>
        /// <param name="operatorString"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ConvertToMaxMinDistance(string operatorString, object value)
        {
            switch (operatorString)
            {
                case Constants.SQLGreaterThanOrEqualSymbol:
                    // ～より距離が長い ⇒ 下限距離を設定
                    return $", '$minDistance' : {value}";
                case Constants.SQLLessThanOrEqualSymbol:
                    // ～より距離が短い ⇒ 上限距離を設定
                    return $", '$maxDistance' : {value}";
                case Constants.SQLEqualSymbol:
                    // ～と距離が等しい ⇒ 下限距離と上限距離を両方設定
                    return $", '$minDistance' : {value} , '$maxDistance' : {value}";
                default:
                    // 上記以外の比較演算子は対応不可
                    throw new ODataNotConvertibleToQueryException($"不正な引数が指定されました。operator : '{operatorString}' 比較演算子は'ge', 'le', 'eq'のいずれかである必要があります。");
            }
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
    }
}
