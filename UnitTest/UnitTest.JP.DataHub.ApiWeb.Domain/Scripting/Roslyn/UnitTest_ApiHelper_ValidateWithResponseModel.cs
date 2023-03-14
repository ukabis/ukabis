using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Scripting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass]
    public class UnitTest_ApiHelper_ValidateWithResponseModel : UnitTestBase
    {
        #region Setup

        private void SetUpContainer()
        {
            base.TestInitialize(true);

            var dataContainer = new PerRequestDataContainer();
            dataContainer.ReturnNeedsJsonValidatorErrorDetail = false;

            UnityContainer.RegisterInstance<IDataContainer>(dataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);
            UnityContainer.RegisterInstance("Return.JsonValidator.ErrorDetail", true);
        }

        #endregion

        [TestMethod]
        public void ValidateWithResponseModel_正常系()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-01'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.ResponseSchema).Returns(new DataSchema(model));
            SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithResponseModel(contents);
            result.IsNull();
        }

        [TestMethod]
        public void ValidateWithResponseModel_異常系_FormatError()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.ResponseSchema).Returns(new DataSchema(model));
            SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithResponseModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["a1"].Single().Is("String '2020-01-32' does not validate against format 'date'.(code:23)");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateWithResponseModel_異常系_model_is_null()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32'}}";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.ResponseSchema).Returns(new DataSchema(null));
            SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithResponseModel(contents);
            result.IsNull();
        }

        [TestMethod]
        public void ValidateWithResponseModel_異常系_ContentsIsNotValidJson()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.ResponseSchema).Returns(new DataSchema(model));
            SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithResponseModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["Unparsable"].Single().Is("Unterminated string. Expected delimiter: '. Path 'a1', line 1, position 18.");
        }
    }
}
