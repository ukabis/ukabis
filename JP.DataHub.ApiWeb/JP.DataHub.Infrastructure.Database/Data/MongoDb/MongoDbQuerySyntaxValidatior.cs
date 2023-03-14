using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using JP.DataHub.Com.Log;
using JP.DataHub.Infrastructure.Database.Resources;

namespace JP.DataHub.Infrastructure.Database.Data.MongoDb
{
    public class MongoDbQuerySyntaxValidatior : IQuerySyntaxValidator
    {
        /// <summary>
        /// APIクエリ構文の検証
        /// </summary>
        public bool Validate(string query, out string message)
        {
            message = string.Empty;

            try
            {
                // パラメータを置換してからBson化
                var queryWithoutParameters = ReplaceParameters(query);
                var bsonQuery = queryWithoutParameters.ToDecimalizedBsonDocument();

                // Aggregate構文チェック
                if (bsonQuery.Contains("Aggregate") && bsonQuery["Aggregate"] != BsonNull.Value)
                {
                    foreach (var property in new string[] { "Select", "Where", "OrderBy", "Top", "Skip" })
                    {
                        if (bsonQuery.Contains(property) && bsonQuery[property] != BsonNull.Value)
                        {
                            message = string.Format(InfrastructureMessages.MongoDbOthersCantSpecifiedWhenAggregate, property);
                            return false;
                        }
                    }

                    var pipeline = bsonQuery["Aggregate"].AsBsonArray;
                    if (!ValidatePipelineSyntax(pipeline, null, out message))
                    {
                        return false;
                    }
                }

                // Where構文チェック
                if (bsonQuery.Contains("Where") && bsonQuery["Where"] != BsonNull.Value)
                {
                    if (!ValidateFindSyntax(bsonQuery["Where"].AsBsonDocument, out message))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex) when (ex is BsonException || ex is KeyNotFoundException)
            {
                message = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                var logger = new JPDataHubLogger(typeof(MongoDbQuerySyntaxValidatior));
                logger.Error(ex.Message, ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Find構文の検証
        /// </summary>
        public virtual bool ValidateFindSyntax(BsonDocument query, out string message)
        {
            message = string.Empty;

            try
            {
                var operatorName = MongoDbConstants.UnusableMongoDbOperators.FirstOrDefault(x => ContainsOperatorRecursive(query, x));
                if (!string.IsNullOrEmpty(operatorName))
                {
                    message = string.Format(InfrastructureMessages.MongoDbUnusableStageOrOperator, operatorName);
                    return false;
                }
            }
            catch (Exception ex) when (ex is BsonException || ex is KeyNotFoundException)
            {
                message = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                var logger = new JPDataHubLogger(typeof(MongoDbQuerySyntaxValidatior));
                logger.Error(ex.Message, ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// パイプライン構文の検証
        /// </summary>
        public virtual bool ValidatePipelineSyntax(BsonArray pipeline, string actualCollectionName, out string message)
        {
            message = string.Empty;

            try
            {
                foreach (var item in pipeline)
                {
                    var stage = item.AsBsonDocument;

                    // 使用禁止ステージのチェック
                    var stageName = MongoDbConstants.UnusableMongoDbAggregationStages.FirstOrDefault(x => stage.Contains(x));
                    if (!string.IsNullOrEmpty(stageName))
                    {
                        message = string.Format(InfrastructureMessages.MongoDbUnusableStageOrOperator, stageName);
                        return false;
                    }

                    // クエリのチェック
                    if (stage.Contains("$match"))
                    {
                        var operatorName = MongoDbConstants.UnusableMongoDbOperators.FirstOrDefault(x => ContainsOperatorRecursive(stage["$match"].AsBsonDocument, x));
                        if (!string.IsNullOrEmpty(operatorName))
                        {
                            message = string.Format(InfrastructureMessages.MongoDbUnusableStageOrOperator, operatorName);
                            return false;
                        }
                    }

                    if (stage.Contains("$geoNear") && stage["$geoNear"].AsBsonDocument.Contains("query"))
                    {
                        var operatorName = MongoDbConstants.UnusableMongoDbOperators.FirstOrDefault(x => ContainsOperatorRecursive(stage["$geoNear"]["query"].AsBsonDocument, x));
                        if (!string.IsNullOrEmpty(operatorName))
                        {
                            message = string.Format(InfrastructureMessages.MongoDbUnusableStageOrOperator, operatorName);
                            return false;
                        }
                    }

                    // UNIONステージのチェック
                    if (stage.Contains("$unionWith"))
                    {
                        // コレクション名指定チェック
                        // 別のリソースの参照を禁止するためコレクション名は変数のみ許可
                        var collectionName = stage["$unionWith"]["coll"].AsString;
                        if (collectionName != MongoDbConstants.CollectionNameVariable &&
                            !(!string.IsNullOrWhiteSpace(actualCollectionName) && collectionName == actualCollectionName))
                        {
                            message = InfrastructureMessages.MongoDbCollectionNameCantSpecified;
                            return false;
                        }

                        // 子要素の再帰チェック
                        if (!ValidatePipelineSyntax(stage["$unionWith"]["pipeline"].AsBsonArray, actualCollectionName, out message))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex) when (ex is BsonException || ex is KeyNotFoundException)
            {
                message = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                var logger = new JPDataHubLogger(typeof(MongoDbQuerySyntaxValidatior));
                logger.Error(ex.Message, ex);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 指定したオペレータを含むか
        /// </summary>
        private bool ContainsOperatorRecursive(BsonDocument document, string operatorName)
        {
            foreach (var element in document)
            {
                if (element.Name == operatorName)
                {
                    return true;
                }

                if (element.Value.IsBsonDocument && ContainsOperatorRecursive(element.Value.AsBsonDocument, operatorName))
                {
                    return true;
                }
                else if (element.Value.IsBsonArray && ContainsOperatorRecursive(element.Value.AsBsonArray, operatorName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 指定したオペレータを含むか
        /// </summary>
        private bool ContainsOperatorRecursive(BsonArray array, string operatorName)
        {
            foreach (var item in array)
            {
                if (item.IsBsonDocument && ContainsOperatorRecursive(item.AsBsonDocument, operatorName))
                {
                    return true;
                }
                else if (item.IsBsonArray && ContainsOperatorRecursive(item.AsBsonArray, operatorName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// パラメータを置換
        /// </summary>
        private static string ReplaceParameters(string targetString)
        {
            var pattern = @"{[-_0-9a-zA-Z]+}";
            new Regex(pattern)
                .Matches(targetString)
                .Cast<Match>()
                .ToList()
                .ForEach(x => {
                    if (x.Value == $"{{{MongoDbConstants.CollectionNameVariable}}}")
                    {
                        targetString = targetString.Replace($"{{{MongoDbConstants.CollectionNameVariable}}}", $"\"{MongoDbConstants.CollectionNameVariable}\"");
                    }
                    else
                    {
                        targetString = targetString.Replace(x.Value, $"\"\"");
                    }
                });

            return targetString;
        }
    }
}
