using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo;

namespace JP.DataHub.ApiWeb.Controllers
{
    /// <summary>
    /// ODataのmatadataを提供します。
    /// </summary>
    [ManageApi("17AAF70A-8170-4ED3-8E54-C2DE4FC4C56B")]
    public class MetadataInfoController : AbstractController
    {
        private const string MediaTypeXml = "application/xml";

        private IMetadataInfoInterface _metadataInfoInterface = UnityCore.Resolve<IMetadataInfoInterface>();


        /// <summary>
        /// metadataを取得します。
        /// </summary>
        /// <returns>
        /// metadata
        /// </returns>
        [HttpGet]
        [Route("$metadata")]
        [ManageAction("754F6077-C26B-40DF-9E2C-500D00362399")]
        public IActionResult Get()
        {
            //Helpページを表示しない設定の場合はNotFoundを返す
            bool invisibleHelpLink = UnityCore.Resolve<IConfiguration>().GetValue<bool>("AppConfig:InvisibleHelpLink", false);
            if (invisibleHelpLink == true)
            {
                return NotFound();
            }

            GetApiSchema(out List<ApiDescriptionModel> apis, out List<SchemaDescriptionModel> schemas, out List<SchemaDescriptionModel> urlSchemas);
            var xmlMetadata = _metadataInfoInterface.CreateMetadata(apis, schemas, urlSchemas);
            return new ContentResult() { StatusCode = (int)HttpStatusCode.OK, Content = xmlMetadata, ContentType = MediaTypeXml };
        }


        private void GetApiSchema(out List<ApiDescriptionModel> apis, out List<SchemaDescriptionModel> schemas, out List<SchemaDescriptionModel> urlSchemas)
        {
            // 全APIを取得
            apis = _metadataInfoInterface.GetApiDescription(false, null, true, true, true).ToList();

            // jsonスキーマの一覧を取得する
            var allSchemas = _metadataInfoInterface.GetSchemaDescription();
            schemas = new List<SchemaDescriptionModel>();
            urlSchemas = new List<SchemaDescriptionModel>();

            // APIと紐付くjsonスキーマだけのListを作る
            foreach (var schema in allSchemas)
            {
                if (apis.Any(x => x.Methods.Any(y => y.RequestSchemaId == schema.SchemaId || y.ResponseSchemaId == schema.SchemaId)))
                {
                    schemas.Add(schema);
                }

                // URLスキーマは別でリストにする
                if (apis.Any(x => x.Methods.Any(y => y.UrlSchemaId == schema.SchemaId)))
                {
                    urlSchemas.Add(schema);
                }
            }
        }
    }
}