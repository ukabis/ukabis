using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.Infrastructure.Database.Data.MongoDb;
using Microsoft.Extensions.Configuration;
using SendGrid;
using System;
using System.Collections.Generic;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace JP.DataHub.ManageApi
{
    [UnityBuildup]
    public class UnityBuildup : IUnityBuildup
    {
        public void Buildup(IUnityContainer container, IConfiguration configuration)
        {
            var appconfig = configuration.GetSection("AppConfig");
            container.RegisterInstance("ManageApiCacheExpirationTimeSpan", appconfig.GetValue<TimeSpan>("ManageApiCacheExpirationTimeSpan", new TimeSpan(0, 30, 0)));
            container.RegisterInstance("UseApiAttachFileDocumentHistory", appconfig.GetValue<bool>("UseApiAttachFileDocumentHistory", true));
            container.RegisterInstance("DynamicApiAttachfileMateDataSchemaId", appconfig.GetValue<string>("DynamicApiAttachfileMateDataSchemaId"));
            container.RegisterInstance("DynamicApiAttachfileCreateResponseDataSchemaId", appconfig.GetValue<string>("DynamicApiAttachfileCreateResponseDataSchemaId"));

            container.RegisterInstance("SendMailTamplateCdDocUserInvitation", appconfig.GetValue<string>("SendMailTamplateCdDocUserInvitation"));
            container.RegisterInstance("UserInvitationSiteUrl", appconfig.GetValue<string>("UserInvitationSiteUrl"));

            container.RegisterType<IQuerySyntaxValidatorFactory, QuerySyntaxValidatorFactory>(new PerResolveLifetimeManager());
            container.RegisterType<IQuerySyntaxValidator, MongoDbQuerySyntaxValidatior>("mng", new PerResolveLifetimeManager());


        }
    }
}