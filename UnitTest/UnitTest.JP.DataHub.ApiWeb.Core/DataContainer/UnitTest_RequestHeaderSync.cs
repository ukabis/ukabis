using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using JP.DataHub.ApiWeb.Core.DataContainer;

namespace UnitTest.JP.DataHub.ApiWeb.Core.DataContainer
{
    [TestClass]
    public class UnitTest_RequestHeaderSync
    {
        [TestMethod]
        public void Constructor_正常系_ヘッダー名がnull()
        {
            AssertEx.Throws<ArgumentException>(() =>
            {
                new RequestHeaderSync<string>(new Dictionary<string, List<string>>(), null);
            });
        }

        [TestMethod]
        public void Constructor_正常系_ヘッダー名が空文字()
        {
            AssertEx.Throws<ArgumentException>(() =>
            {
                new RequestHeaderSync<string>(new Dictionary<string, List<string>>(), "");
            });
        }

        [TestMethod]
        public void GetValue_正常系_headersに存在する項目()
        {
            var headers = new Dictionary<string, List<string>>()
            {
                { "X-StringHeader", new List<string>() { "value" } },
                { "X-IntegerHeader", new List<string>() { "1" } },
            };
            var stringSync = new RequestHeaderSync<string>(headers, "X-StringHeader");
            var intSync = new RequestHeaderSync<int>(headers, "X-IntegerHeader");

            // headersの値を取得できるかどうか
            stringSync.GetValue().Is("value");
            intSync.GetValue().Is(1);
        }

        [TestMethod]
        public void GetValue_正常系_headersに存在しない項目()
        {
            var headers = new Dictionary<string, List<string>>();
            var stringSync = new RequestHeaderSync<string>(headers, "X-StringHeader");
            var intSync = new RequestHeaderSync<int>(headers, "X-IntegerHeader");

            // 型のデフォルト値を取得できるかどうか
            stringSync.GetValue().IsNull();
            intSync.GetValue().Is(0);
        }

        [TestMethod]
        public void GetValue_正常系_headersがnull()
        {
            Dictionary<string, List<string>> headers = null;
            var stringSync = new RequestHeaderSync<string>(headers, "X-StringHeader");
            var intSync = new RequestHeaderSync<int>(headers, "X-IntegerHeader");

            // 型のデフォルト値を取得できるかどうか
            stringSync.GetValue().IsNull();
            intSync.GetValue().Is(0);
        }

        [TestMethod]
        public void SetValue_正常系_headersに存在する項目()
        {
            var headers = new Dictionary<string, List<string>>()
            {
                { "X-StringHeader", new List<string>() { "value" } },
                { "X-IntegerHeader", new List<string>() { "1" } },
            };
            var stringSync = new RequestHeaderSync<string>(headers, "X-StringHeader");
            var intSync = new RequestHeaderSync<int>(headers, "X-IntegerHeader");

            stringSync.SetValue("changed");
            intSync.SetValue(2);

            // 設定した値がheadersに反映されているかどうか
            headers["X-StringHeader"].Single().Is("changed");
            headers["X-IntegerHeader"].Single().Is("2");
        }

        [TestMethod]
        public void SetValue_正常系_headersに存在しない項目()
        {
            var headers = new Dictionary<string, List<string>>();
            var stringSync = new RequestHeaderSync<string>(headers, "X-StringHeader");
            var intSync = new RequestHeaderSync<int>(headers, "X-IntegerHeader");

            stringSync.SetValue("changed");
            intSync.SetValue(2);

            // 設定した値がheadersに追加されているかどうか
            headers["X-StringHeader"].Single().Is("changed");
            headers["X-IntegerHeader"].Single().Is("2");
        }

        [TestMethod]
        public void SetValue_異常系_headersがnull()
        {
            Dictionary<string, List<string>> headers = null;
            var stringSync = new RequestHeaderSync<string>(headers, "X-StringHeader");
            var intSync = new RequestHeaderSync<int>(headers, "X-IntegerHeader");

            AssertEx.Throws<NullReferenceException>(() =>
            {
                stringSync.SetValue("changed");
            });

            AssertEx.Throws<NullReferenceException>(() =>
            {
                intSync.SetValue(2);
            });
        }
    }
}
