using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class GetDocumentVersionAction : QueryAction
    {
        #region Mapper
        private static Lazy<IMapper> _Mapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DocumentHistorySnapshot, GetDocumentVersionResultSnapshot>();
                cfg.CreateMap<DocumentHistory, GetDocumentVersionResult>()
                    .ForMember(dst => dst.Snapshot, opt => {
                        opt.Condition((src) => src.Snapshot != null);
                        opt.MapFrom(src => src.Snapshot);
                    });
            });
            return mappingConfig.CreateMapper();
        });
        private static IMapper Mapper { get => _Mapper.Value; }

        #endregion


        public override HttpResponseMessage ExecuteAction()
        {
            if ((this.MethodType.IsGet != true) && (this.MethodType.IsPost != true) && (this.MethodType.IsPut != true))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30410, this.RelativeUri?.Value);
            }

            if (EnableJsonDocumentHistory == false || IsDocumentHistory?.Value != true)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30501, this.RelativeUri?.Value);
            }

            if (Query.ContainKey("id") == false)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30402, this.RelativeUri?.Value);
            }
            var ver = DynamicApiDataStoreRepository[0].DocumentVersionRepository.GetDocumentVersion(new DocumentKey(this.RepositoryKey, Query.GetValue("id")));
            if (ver == null)
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30411, this.RelativeUri?.Value);
            }

            //idチェックを実施し、アクセス可能かどうかチェック
            //このActionは、GetDocumentVersionとGetAttachFileDocumentVersion からアクセスが来るため
            //AttachFileはDocumentIdForAttachFile、GetDocumentVersionはId を使う
            var documentId = ver.DocumentIdForAttachFile == null ? ver.Id : ver.DocumentIdForAttachFile;
            if (string.IsNullOrEmpty(documentId))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }
            else if (!CheckRequestIdIsValid(documentId))
            {
                return ErrorCodeMessage.GetRFC7807HttpResponseMessage(ErrorCodeMessage.Code.E30404, this.RelativeUri?.Value);
            }
            var x = Mapper.Map<List<GetDocumentVersionResult>>(ver.DocumentVersions);
            return TupleToHttpResponseMessage(Tuple.Create<HttpStatusCode, string>(HttpStatusCode.OK, x?.ToJson().ToString()));
        }
    }
}
