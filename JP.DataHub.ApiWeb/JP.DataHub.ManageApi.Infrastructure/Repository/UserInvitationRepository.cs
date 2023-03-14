using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using JP.DataHub.Infrastructure.Database.Authority;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ManageApi.Infrastructure.Repository
{
    internal class UserInvitationRepository : AbstractRepository, IUserInvitationRepository
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SendUserInvitationModel, DB_UserInvitation>()
                    .ForMember(dst => dst.user_invitation_id, ops => ops.Ignore())
                    .ForMember(dst => dst.mailaddress, ops => ops.MapFrom(src => src.MailAddress))
                    .ForMember(dst => dst.vendor_id, ops => ops.MapFrom(src => src.VendorId))
                    .ForMember(dst => dst.invitation_date, ops => ops.Ignore())
                    .ForMember(dst => dst.register_staff_id, ops => ops.Ignore())
                    .ForMember(dst => dst.register_account, ops => ops.Ignore())
                    .ForMember(dst => dst.is_used, ops => ops.MapFrom(src => false))
                    .ForMember(dst => dst.is_active, ops => ops.MapFrom(src => false))
                    .ForMember(dst => dst.role_id, ops => ops.MapFrom(src => src.RoleId))
                    .ForMember(dst => dst.staff_id, ops => ops.Ignore())
                    .AfterMap((src, dst) => dst.UpdateFiveColumn());
                cfg.CreateMap<DB_UserInvitation, UserInvitationModel>();
            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }

        private Lazy<IJPDataHubDbConnection> _lazyConnection = new Lazy<IJPDataHubDbConnection>(() => UnityCore.Resolve<IJPDataHubDbConnection>("Authority"));
        private IJPDataHubDbConnection _connection { get => _lazyConnection.Value; }

        public string RegisterUserInvitation(SendUserInvitationModel model)
        {
            var userInvitation = s_mapper.Map<DB_UserInvitation>(model);
            userInvitation.invitation_date = UtcNow;
            userInvitation.expire_date = UtcNow.AddHours(Convert.ToDouble(s_appConfig.GetValue<int>("UserInvitationExpireDate",1)));
            return _connection.Insert(userInvitation).ToString();
        }

        public UserInvitationModel Get(string user_invitation_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    user_invitation_id AS UserInvitationId
    ,mailaddress AS MailAddress
    ,vendor_id AS VendorId
    ,invitation_date AS InvitationDate
    ,expire_date AS ExpireDate
    ,register_staff_id AS RegisterStaffId
    ,register_account AS RegisterAccount
    ,is_used AS IsUsed
    ,role_id AS RoleId
    ,staff_id AS StaffId
FROM
    USER_INVITATION
WHERE
    user_invitation_id= /*ds user_invitation_id*/'1' 
    AND is_active=1
";
            }
            else
            {
                sql = @"
SELECT
    user_invitation_id AS UserInvitationId
    ,mailaddress AS MailAddress
    ,vendor_id AS VendorId
    ,invitation_date AS InvitationDate
    ,expire_date AS ExpireDate
    ,register_staff_id AS RegisterStaffId
    ,register_account AS RegisterAccount
    ,is_used AS IsUsed
    ,role_id AS RoleId
    ,staff_id AS StaffId
FROM
    UserInvitation
WHERE
    user_invitation_id=@user_invitation_id
    AND is_active=1
";
            }
            var param = new { user_invitation_id = user_invitation_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return _connection.QuerySingle<UserInvitationModel>(twowaySql.Sql, dynParams);
        }

        public void Invited(string user_invitation_id)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";

            if (dbSettings.Type == "Oracle")
            {
                sql = "UPDATE USER_INVITATION SET is_used=1 WHERE user_invitation_id= /*ds user_invitation_id*/'1' AND is_active=1 AND is_used=0";
            }
            else
            {
                sql = "UPDATE UserInvitation SET is_used=1 WHERE user_invitation_id=@user_invitation_id AND is_active=1 AND is_used=0";
            }
            var param = new { user_invitation_id = user_invitation_id };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _connection.ExecutePrimaryKey(twowaySql.Sql, dynParams);
        }
    }
}
