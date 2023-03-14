using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;
using JP.DataHub.ApiWeb.Models.OpenIdUser;

namespace JP.DataHub.ApiWeb.Controllers
{
    /// <summary>
    /// 外部システムとログインユーザーを同期するためのAPIを提供します。
    /// </summary>
    [ApiController]
    [Route("API/Users/[action]")]
    [ManageApi("981f3eaa-0060-481b-ae96-a860219211cf")]
    [AuthorizeUsingOpenIdConnect]
    public class OpenIdUserController : AbstractController
    {
        private const string SystemIdErrorMessage = "Can not get SystemId.";

        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OpenIdUserModel, OpenIdUserViewModel>();
                cfg.CreateMap<OpenIdUserRequestViewModel, OpenIdUserModel>();
            });

            return config.CreateMapper();
        });
        private static IMapper s_mapper => s_lazyMapper.Value;

        private IOpenIdUserInterface _openIdUserInterface = UnityCore.Resolve<IOpenIdUserInterface>();


        /// <summary>
        /// 自システムで登録されたユーザーの一覧を返します。
        /// </summary>
        /// <returns>ユーザーの一覧</returns>
        [HttpGet]
        [ManageAction("e9d31c29-0243-4eb3-8321-031c0055ec90")]
        public async Task<IActionResult> GetAll()
        {
            if (string.IsNullOrEmpty(PerRequestDataContainer?.SystemId))
            {
                return BadRequest(SystemIdErrorMessage);
            }

            try
            {
                var users = await _openIdUserInterface.GetList(PerRequestDataContainer.SystemId);
                var result = s_mapper.Map<IEnumerable<OpenIdUserViewModel>>(users);
                return Ok(result);
            }
            catch (OpenIdUserOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 指定されたユーザーID（メールアドレス）のユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>OpenIDユーザー</returns>
        [HttpGet]
        [ManageAction("eda49593-498f-4ef2-9d0b-ee248c7481d3")]
        public async Task<IActionResult> Get(string userId)
        {
            if (string.IsNullOrEmpty(PerRequestDataContainer?.SystemId))
            {
                return BadRequest(SystemIdErrorMessage);
            }

            try
            {
                var operationResult = await _openIdUserInterface.Get(PerRequestDataContainer.SystemId, userId);
                if (operationResult.statusCode == HttpStatusCode.OK)
                {
                    var result = s_mapper.Map<OpenIdUserViewModel>(operationResult.userInfo);
                    return result != null ? Ok(result) : NotFound();
                }
                else
                {
                    return StatusCode((int)operationResult.statusCode);
                }
            }
            catch (OpenIdUserOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 指定されたユーザーID（メールアドレス）のユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>OpenIDユーザー</returns>
        [HttpGet]
        [ManageAction("9C88D4FB-C1E7-4BDF-B15D-C59D0356F52A")]
        [Admin]
        public async Task<IActionResult> GetFullAccess(string userId)
        {
            try
            {
                var operationResult = await _openIdUserInterface.GetFullAccess(userId);
                if (operationResult.statusCode == HttpStatusCode.OK)
                {
                    var result = s_mapper.Map<OpenIdUserViewModel>(operationResult.userInfo);
                    return result != null ? Ok(result) : NotFound();
                }
                else
                {
                    return StatusCode((int)operationResult.statusCode);
                }
            }
            catch (OpenIdUserOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 指定されたユーザー情報でユーザーを登録します。
        /// </summary>
        /// <param name="user">OpenIDユーザー情報</param>
        /// <returns>登録結果</returns>
        [HttpPost]
        [ManageAction("95d5f2d7-c93e-4451-b84c-4917060412ea")]
        public async Task<IActionResult> Post(OpenIdUserRequestViewModel user)
        {
            if (string.IsNullOrEmpty(PerRequestDataContainer?.SystemId))
            {
                return BadRequest(SystemIdErrorMessage);
            }

            if (ModelState.IsValid)
            {
                // 登録データを作成
                var request = s_mapper.Map<OpenIdUserModel>(user);

                try
                {
                    // ユーザーを登録
                    var operationResult = await _openIdUserInterface.Register(PerRequestDataContainer?.SystemId, request);

                    if (operationResult.statusCode == HttpStatusCode.Created)
                    {
                        // 返却データを作成
                        var result = s_mapper.Map<OpenIdUserViewModel>(operationResult.userInfo);
                        return Created(string.Empty, result);
                    }
                    else
                    {
                        return StatusCode((int)operationResult.statusCode);
                    }
                }
                catch (OpenIdUserOperationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        /// <summary>
        /// 指定されたユーザーID（メールアドレス）のユーザーを削除します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>削除結果</returns>
        [HttpDelete]
        [ManageAction("1a6ad4f6-9053-4729-af8c-5c2bfd92a4c4")]
        public async Task<IActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(PerRequestDataContainer?.SystemId))
            {
                return BadRequest(SystemIdErrorMessage);
            }

            try
            {
                var result = await _openIdUserInterface.Delete(PerRequestDataContainer.SystemId, userId);
                return StatusCode((int)result);
            }
            catch (OpenIdUserOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}