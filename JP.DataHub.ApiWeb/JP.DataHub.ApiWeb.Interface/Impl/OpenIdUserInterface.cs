using System.Net;
using AutoMapper;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.OpenIdUser;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Interface.Impl
{
    /// <summary>
    /// OpenIdユーザー管理のためのインターフェースの実装です。
    /// </summary>
    class OpenIdUserInterface : IOpenIdUserInterface
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OpenIdUser, OpenIdUserModel>()
                    .ForMember(d => d.UserId, o => o.MapFrom(s => s.UserId.Value))
                    .ForMember(d => d.OpenId, o => o.MapFrom(s => s.ObjectId.Value));
            });

            return config.CreateMapper();
        });
        private static IMapper s_mapper => s_lazyMapper.Value;

        private IOpenIdUserApplicationService _service = UnityCore.Resolve<IOpenIdUserApplicationService>();


        /// <summary>
        /// 指定されたユーザーIDのユーザーを削除します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>削除結果</returns>
        public async Task<HttpStatusCode> Delete(string systemId, string userId)
            => ToHttpStatusCode(await _service.Delete(new SystemId(systemId), new UserId(userId)));

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        public async Task<(HttpStatusCode statusCode, OpenIdUserModel userInfo)> Get(string systemId, string userId)
        {
            // 既存ユーザーを取得
            var operationResult = await _service.Get(new SystemId(systemId), new UserId(userId));
            var userModel = s_mapper.Map<OpenIdUserModel>(operationResult.UserInfo);

            return (ToHttpStatusCode(operationResult), userModel);
        }

        /// <summary>
        /// 指定されたユーザーIDのユーザー情報を返します。
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <returns>ユーザー情報</returns>
        public async Task<(HttpStatusCode statusCode, OpenIdUserModel userInfo)> GetFullAccess(string userId)
        {
            // 既存ユーザーを取得
            var operationResult = await _service.GetFullAccess(new UserId(userId));
            var userModel = s_mapper.Map<OpenIdUserModel>(operationResult.UserInfo);

            return (ToHttpStatusCode(operationResult), userModel);
        }

        /// <summary>
        /// 指定されたシステムで登録されたユーザーの一覧を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>ユーザーの一覧</returns>
        public async Task<IEnumerable<OpenIdUserModel>> GetList(string systemId)
        {
            // ユーザーの一覧をを取得
            var users = await _service.GetList(new SystemId(systemId));

            return s_mapper.Map<IEnumerable<OpenIdUserModel>>(users);
        }

        /// <summary>
        /// 指定されたユーザー情報でユーザーを登録します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="user">ユーザー情報</param>
        /// <returns>登録結果</returns>
        public async Task<(HttpStatusCode statusCode, OpenIdUserModel userInfo)> Register(string systemId, OpenIdUserModel user)
        {
            var oidUser = new OpenIdUser(null, user.UserId, systemId, user.Password, user.UserName, DateTime.MinValue);
            // ユーザーを登録
            var operationResult = await _service.Register(oidUser);

            var userModel = s_mapper.Map<OpenIdUserModel>(operationResult.UserInfo);

            return (ToHttpStatusCode(operationResult), userModel);
        }

        /// <summary>
        /// OpenIdUserOperationResultをHttpStatusCodeに変換します。
        /// </summary>
        /// <param name="operationResult">OpenIdUserOperationResult</param>
        /// <returns>HttpStatusCode</returns>
        private HttpStatusCode ToHttpStatusCode(OpenIdUserOperationResult operationResult)
        {
            if (operationResult.Status == OpenIdUserOperationStatus.Selected)
                return HttpStatusCode.OK;
            else if (operationResult.Status == OpenIdUserOperationStatus.Created)
                return HttpStatusCode.Created;
            else if (operationResult.Status == OpenIdUserOperationStatus.Updated)
                return HttpStatusCode.NoContent;
            else if (operationResult.Status == OpenIdUserOperationStatus.Deleted)
                return HttpStatusCode.NoContent;
            else if (operationResult.Status == OpenIdUserOperationStatus.NotFound)
                return HttpStatusCode.NotFound;
            else if (operationResult.Status == OpenIdUserOperationStatus.Forbidden)
                return HttpStatusCode.Forbidden;
            else return HttpStatusCode.BadRequest;
        }
    }
}
