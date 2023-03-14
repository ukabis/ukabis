using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Misc;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_ApiTreeNode : UnitTestBase
    {

        [TestMethod]
        public void ApiTreeNode_AddApiEntity_空に追加()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(1);
            target.Api.Count.Is(0);

            var nextNode = target.NextNode[0];
            nextNode.RelativePath.Is(expectControllerUrl);
            nextNode.NextNode.Count.Is(0);
            nextNode.Api.Count.Is(1);
            nextNode.Api[0].IsSameReferenceAs(entityIdentifier);
        }

        [TestMethod]
        public void ApiTreeNode_AddApiEntity_空に追加_URLネスト()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(1);
            target.Api.Count.Is(0);

            var nextNode = target.NextNode[0];
            nextNode.RelativePath.Is(expectControllerUrl);
            nextNode.NextNode.Count.Is(1);
            nextNode.Api.Count.Is(0);

            var nextNode2 = nextNode.NextNode[0];
            nextNode2.RelativePath.Is(expectControllerUrl2);
            nextNode2.NextNode.Count.Is(0);
            nextNode2.Api.Count.Is(1);
            nextNode2.Api[0].IsSameReferenceAs(entityIdentifier);
        }

        [TestMethod]
        public void ApiTreeNode_AddApiEntity_データ1つ有に追加_既存データと別URL()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}"
            );

            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier2 = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);
            target.AddApiEntity(entityIdentifier2);

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(2);
            target.Api.Count.Is(0);

            var nextNode = target.NextNode[0];
            nextNode.RelativePath.Is(expectControllerUrl);
            nextNode.NextNode.Count.Is(0);
            nextNode.Api.Count.Is(1);
            nextNode.Api[0].IsSameReferenceAs(entityIdentifier);

            var nextNode2 = target.NextNode[1];
            nextNode2.RelativePath.Is(expectControllerUrl2);
            nextNode2.NextNode.Count.Is(0);
            nextNode2.Api.Count.Is(1);
            nextNode2.Api[0].IsSameReferenceAs(entityIdentifier2);
        }

        [TestMethod]
        public void ApiTreeNode_AddApiEntity_データ1つ有に追加_既存データの子URL()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}"
            );

            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier2 = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);
            target.AddApiEntity(entityIdentifier2);

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(1);
            target.Api.Count.Is(0);

            var nextNode = target.NextNode[0];
            nextNode.RelativePath.Is(expectControllerUrl);
            nextNode.NextNode.Count.Is(1);
            nextNode.Api.Count.Is(1);
            nextNode.Api[0].IsSameReferenceAs(entityIdentifier);

            var nextNode2 = nextNode.NextNode[0];
            nextNode2.RelativePath.Is(expectControllerUrl2);
            nextNode2.NextNode.Count.Is(0);
            nextNode2.Api.Count.Is(1);
            nextNode2.Api[0].IsSameReferenceAs(entityIdentifier2);
        }

        [TestMethod]
        public void ApiTreeNode_Remove_削除_ルート()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);
            target.Remove(entityIdentifier.api_id);

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(1);
            target.Api.Count.Is(0);

            var nextNode = target.NextNode[0];
            nextNode.RelativePath.Is(expectControllerUrl);
            nextNode.NextNode.Count.Is(0);
            nextNode.Api.Count.Is(0);
        }

        [TestMethod]
        public void ApiTreeNode_Remove_削除_ルート_2データのうち1つ()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}"
            );

            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier2 = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);
            target.AddApiEntity(entityIdentifier2);
            target.Remove(entityIdentifier.api_id);

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(2);
            target.Api.Count.Is(0);

            var nextNode = target.NextNode[0];
            nextNode.RelativePath.Is(expectControllerUrl);
            nextNode.NextNode.Count.Is(0);
            nextNode.Api.Count.Is(0);

            var nextNode2 = target.NextNode[1];
            nextNode2.RelativePath.Is(expectControllerUrl2);
            nextNode2.NextNode.Count.Is(0);
            nextNode2.Api.Count.Is(1);
            nextNode2.Api[0].IsSameReferenceAs(entityIdentifier2);
        }

        [TestMethod]
        public void ApiTreeNode_Remove_削除_子要素()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}"
            );

            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier2 = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);
            target.AddApiEntity(entityIdentifier2);
            target.Remove(entityIdentifier2.api_id);

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(1);
            target.Api.Count.Is(0);

            var nextNode = target.NextNode[0];
            nextNode.RelativePath.Is(expectControllerUrl);
            nextNode.NextNode.Count.Is(1);
            nextNode.Api.Count.Is(1);
            nextNode.Api[0].IsSameReferenceAs(entityIdentifier);

            var nextNode2 = nextNode.NextNode[0];
            nextNode2.RelativePath.Is(expectControllerUrl2);
            nextNode2.NextNode.Count.Is(0);
            nextNode2.Api.Count.Is(0);
        }

        [TestMethod]
        public void ApiTreeNode_Remove_削除なし_一致無し()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);
            target.Remove(Guid.NewGuid());

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(1);
            target.Api.Count.Is(0);

            var nextNode = target.NextNode[0];
            nextNode.RelativePath.Is(expectControllerUrl);
            nextNode.NextNode.Count.Is(0);
            nextNode.Api.Count.Is(1);
            nextNode.Api[0].IsSameReferenceAs(entityIdentifier);
        }

        [TestMethod]
        public void ApiTreeNode_Remove_削除なし_一致無し_データ無し()
        {
            var target = new ApiTreeNode();

            target.Remove(Guid.NewGuid());

            target.RelativePath.IsSameReferenceAs(null);
            target.NextNode.Count.Is(0);
            target.Api.Count.Is(0);
        }

        [TestMethod]
        public void ApiTreeNode_FindApiIdentifier_hit_親要素()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);

            var requestRelativeUri = $"{entityIdentifier.controller_relative_url}/{entityIdentifier.method_name}";
            string normalizedRelativeUri = new RelativeUri(requestRelativeUri).NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);

            var result = target.FindApiIdentifier(
                normalizedRelativeUri,
                splitUrl,
                new HttpMethodType(HttpMethodType.MethodTypeEnum.POST),
                null
            );

            result.IsSameReferenceAs(entityIdentifier);
        }

        [TestMethod]
        public void ApiTreeNode_FindApiIdentifier_hit_子要素()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);

            var requestRelativeUri = $"{entityIdentifier.controller_relative_url}/{entityIdentifier.method_name}";
            string normalizedRelativeUri = new RelativeUri(requestRelativeUri).NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);

            var result = target.FindApiIdentifier(
                normalizedRelativeUri,
                splitUrl,
                new HttpMethodType(HttpMethodType.MethodTypeEnum.POST),
                null
            );

            result.IsSameReferenceAs(entityIdentifier);
        }

        [TestMethod]
        public void ApiTreeNode_FindApiIdentifier_hit無し_URL違い()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);

            var requestRelativeUri = $"API/{expectControllerUrl}/{Guid.NewGuid().ToString()}/{entityIdentifier.method_name}";
            string normalizedRelativeUri = new RelativeUri(requestRelativeUri).NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);

            var result = target.FindApiIdentifier(
                normalizedRelativeUri,
                splitUrl,
                new HttpMethodType(HttpMethodType.MethodTypeEnum.POST),
                null
            );

            result.IsSameReferenceAs(null);
        }

        [TestMethod]
        public void ApiTreeNode_FindApiIdentifier_hit無し_URL違い_検索対象のネストが深い()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);

            var requestRelativeUri = $"API/{expectControllerUrl}/{expectControllerUrl2}/{Guid.NewGuid().ToString()}/{entityIdentifier.method_name}";
            string normalizedRelativeUri = new RelativeUri(requestRelativeUri).NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);

            var result = target.FindApiIdentifier(
                normalizedRelativeUri,
                splitUrl,
                new HttpMethodType(HttpMethodType.MethodTypeEnum.POST),
                null
            );

            result.IsSameReferenceAs(null);
        }

        [TestMethod]
        public void ApiTreeNode_FindApiIdentifier_hit無し_IsMatch不一致()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            target.AddApiEntity(entityIdentifier);

            var requestRelativeUri = $"API/{expectControllerUrl}/{expectControllerUrl2}/{Guid.NewGuid().ToString()}";
            string normalizedRelativeUri = new RelativeUri(requestRelativeUri).NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);

            var result = target.FindApiIdentifier(
                normalizedRelativeUri,
                splitUrl,
                new HttpMethodType(HttpMethodType.MethodTypeEnum.POST),
                null
            );

            result.IsSameReferenceAs(null);
        }

        [TestMethod]
        public void ApiTreeNode_FindApiIdentifier_hit無し_データ無し()
        {
            var expectControllerUrl = Guid.NewGuid().ToString();
            var expectControllerUrl2 = Guid.NewGuid().ToString();
            var entityIdentifier = GetAllApiEntityIdentifier(
                $"API/{expectControllerUrl}/{expectControllerUrl2}"
            );

            var target = new ApiTreeNode();

            var requestRelativeUri = $"API/{expectControllerUrl}/{expectControllerUrl2}/{entityIdentifier.method_name}";
            string normalizedRelativeUri = new RelativeUri(requestRelativeUri).NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);

            var result = target.FindApiIdentifier(
                normalizedRelativeUri,
                splitUrl,
                new HttpMethodType(HttpMethodType.MethodTypeEnum.POST),
                null
            );

            result.IsSameReferenceAs(null);
        }

        private AllApiEntityIdentifier GetAllApiEntityIdentifier(
            string controllerUrl = null,
            string methodName = null,
            string methodType = null,
            string action_type_cd = null,
            string aliasMethodName = null,
            bool? isNomatchQueryString = null
        )
        {
            return new AllApiEntityIdentifier()
            {
                api_id = Guid.NewGuid(),
                controller_relative_url = controllerUrl ?? $"API/{Guid.NewGuid().ToString()}",

                method_name = methodName ?? Guid.NewGuid().ToString(),
                method_type = methodType ?? HttpMethodType.MethodTypeEnum.POST.ToString().ToLower(),
                alias_method_name = aliasMethodName ?? Guid.NewGuid().ToString(),
                is_nomatch_querystring = isNomatchQueryString ?? false,
                action_type_cd = action_type_cd ?? ActionType.Query.ToCode()
            };
        }
    }
}
