using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;
using JP.DataHub.ApiWeb.Models.AttachFile;

namespace JP.DataHub.ApiWeb.Controllers
{
    /// <summary>
    /// 添付ファイルの管理を行います。
    /// </summary>
    [ApiController]
    [Route("API/[controller]/[action]")]
    [ManageApi("72833834-6339-42BB-9625-99578D410C67")]
    [AuthorizeUsingOpenIdConnect]
    public class AttachFileController : AbstractController
    {
        private static readonly string s_contentRange = "Content-Range";
        private static readonly bool s_isInternalServerErrorDetailResponse = UnityCore.Resolve<bool>("IsInternalServerErrorDetailResponse");

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<MetaViewModel, AttachFileMeta>().ReverseMap();
                cfg.CreateMap<CreateFileViewModel, AttachFileModel>();
                cfg.CreateMap<FileMetaViewModel, AttachFileModel>().ReverseMap();
                cfg.CreateMap<FileInfoViewModel, AttachFileModel>().ReverseMap();
            });

            return mappingConfig.CreateMapper();
        });
        private static IMapper s_mapper => s_lazyMapper.Value;

        private IAttachFileInterface AttachFileInterface = UnityCore.Resolve<IAttachFileInterface>();


        /// <summary>
        /// 添付ファイルの情報を作成します。
        /// </summary>
        /// <returns>ファイルID</returns>
        [HttpPost]
        [ManageAction("9D70A4DA-3C47-4280-B516-FA55DF9391EF")]
        public IActionResult CreateFile(CreateFileViewModel model)
        {
            var attachFileModel = s_mapper.Map<AttachFileModel>(model);
            var vendorId = PerRequestDataContainer.VendorId;
            if (string.IsNullOrEmpty(vendorId))
            {
                return new ForbidResult();
            }

            try
            {
                return new OkObjectResult(new CreateFileResultViewModel() { FileId = AttachFileInterface.CreateFile(attachFileModel, true) });
            }
            catch (ApiException de)
            {
                return ToActionResult(new DynamicApiResponse(de.HttpResponseMessage));
            }
            catch (Exception e)
            {
                if (s_isInternalServerErrorDetailResponse)
                {
                    return new ContentResult() { StatusCode = (int)HttpStatusCode.InternalServerError, Content = e.Message };
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 添付ファイルをアップロードします。
        /// </summary>
        /// <param name="FileId">
        /// ファイルID(/API/AttachFile/CreateFileで事前にIDを発行する)
        /// </param>
        /// <param name="File">
        /// 添付ファイルをバイナリで送信する。
        /// 1度に送信できる容量は10MBまで。
        /// それ以上の容量のデータを送信したい場合はContent-Rangeにて送信内容をチャンクしながらアップロードを行う。
        /// </param>
        /// <returns></returns>
        [HttpPost]
        [ManageAction("BAF31CA7-E44D-4326-812F-68DD4522D2C4")]
        public IActionResult UploadFile(string FileId)
        {
            if (string.IsNullOrEmpty(FileId) || !Guid.TryParse(FileId, out _))
            {
                return new BadRequestObjectResult(DynamicApiMessages.FileIdIsInvalid);
            }
            if (Request.ContentLength <= 0)
            {
                return new BadRequestObjectResult(DynamicApiMessages.FileIsRequired);
            }

            ContentRangeHeaderValue range = null;
            if (Request.Headers.ContainsKey(s_contentRange))
            {
                range = ContentRangeHeaderValue.Parse(Request.Headers[s_contentRange]);
            }

            bool isBlobUpload = (range == null) || (range.To == range.Length - 1);
            bool isAppendStream = (range != null) && (range.From != 0);
            long appendPosition = 0;
            if (range != null)
            {
                appendPosition = range.From ?? 0;
            }

            var fileStream = new MemoryStream();
            Request.BodyReader.AsStream().CopyTo(fileStream);

            var model = new AttachFileUploadFileModel()
            {
                FileId = FileId,
                InputStream = fileStream
            };

            try
            {
                AttachFileInterface.UploadFile(model, isBlobUpload, isAppendStream, appendPosition, true);
                return new OkResult();

            }
            catch (ApiException de)
            {
                return ToActionResult(new DynamicApiResponse(de.HttpResponseMessage));
            }
            catch (Exception e)
            {
                if (s_isInternalServerErrorDetailResponse)
                {
                    return new ContentResult() { StatusCode = (int)HttpStatusCode.InternalServerError, Content = e.Message };
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// アップロードした添付ファイルを検索します。
        /// MetaKey1＝MetaValue1＆MetaKey2＝MetaValue2の形式で検索対象となる添付ファイルのメタをGETパラメータとして指定する。
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("BCC7E83A-7492-4E39-AA19-8E0F984B3DDD")]
        public IActionResult MetaQuery()
        {
            Dictionary<string, string> metaDictionary = new Dictionary<string, string>();
            foreach (var q in Request.Query)
            {
                metaDictionary.Add(q.Key, q.Value);
            }

            if (metaDictionary.Count < 1)
            {
                return new BadRequestResult();
            }

            try
            {
                var result = AttachFileInterface.SearchByMeta(metaDictionary);
                return new OkObjectResult(s_mapper.Map<List<FileInfoViewModel>>(result));
            }
            catch (ApiException de)
            {
                return ToActionResult(new DynamicApiResponse(de.HttpResponseMessage));
            }
            catch (Exception e)
            {
                if (s_isInternalServerErrorDetailResponse)
                {
                    return new ContentResult() { StatusCode = (int)HttpStatusCode.InternalServerError, Content = e.Message };
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// ファイルイメージを取得します。
        /// </summary>
        /// <param name="FileId">ファイルID</param>
        /// <param name="Key">Key</param>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("088D03A4-202D-485A-972D-460B8BC55155")]
        public IActionResult GetImage(string FileId, string Key = null)
        {
            Guid fileId;
            if (string.IsNullOrEmpty(FileId) || !Guid.TryParse(FileId, out fileId))
            {
                return new BadRequestResult();
            }

            try
            {
                return ToActionResult(AttachFileInterface.GetFile(FileId, Key, true));
            }
            catch (ApiException de)
            {
                return ToActionResult(new DynamicApiResponse(de.HttpResponseMessage));
            }
            catch (NotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception e)
            {
                if (s_isInternalServerErrorDetailResponse)
                {
                    return new ContentResult() { StatusCode = (int)HttpStatusCode.InternalServerError, Content = e.Message };
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 添付ファイルのメタ情報を取得します。
        /// </summary>
        /// <param name="FileId">ファイルID</param>
        /// <param name="Key">Key</param>
        /// <returns></returns>
        [HttpGet]
        [ManageAction("AE509B33-3F6F-4C4E-923E-67D5237AE61A")]
        public IActionResult GetMeta(string FileId, string Key = null)
        {
            Guid fileId;
            if (string.IsNullOrEmpty(FileId) || !Guid.TryParse(FileId, out fileId))
            {
                return new BadRequestResult();
            }

            try
            {
                var result = AttachFileInterface.GetFileMeta(FileId, Key);
                return new OkObjectResult(s_mapper.Map<FileMetaViewModel>(result));
            }
            catch (ApiException de)
            {
                return ToActionResult(new DynamicApiResponse(de.HttpResponseMessage));
            }
            catch (Exception e)
            {
                if (s_isInternalServerErrorDetailResponse)
                {
                    return new ContentResult() { StatusCode = (int)HttpStatusCode.InternalServerError, Content = e.Message };
                }
                else
                {
                    throw;
                }
            }

        }

        /// <summary>
        /// 添付ファイルを削除します。
        /// </summary>
        /// <param name="FileId">ファイルID</param>
        /// <param name="Key">Key</param>
        /// <returns></returns>
        [HttpDelete]
        [ManageAction("3AA63E54-523A-4283-AE87-24A12AA91A4C")]
        public IActionResult Delete(string FileId, string Key = null)
        {
            Guid fileId;
            if (string.IsNullOrEmpty(FileId) || !Guid.TryParse(FileId, out fileId))
            {
                return new BadRequestResult();
            }
            try
            {
                AttachFileInterface.DeleteFile(FileId, Key, true);
                return new NoContentResult();
            }
            catch (ApiException de)
            {
                return ToActionResult(new DynamicApiResponse(de.HttpResponseMessage));
            }
            catch (Exception e)
            {
                if (s_isInternalServerErrorDetailResponse)
                {
                    return new ContentResult() { StatusCode = (int)HttpStatusCode.InternalServerError, Content = e.Message };
                }
                else
                {
                    throw;
                }
            }
        }
    }
}