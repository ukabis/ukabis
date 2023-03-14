using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Logging;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    [TestClass]
    public class UnitTest_LoggingStream
    {
        [TestMethod]
        public void LoggingStream_通常()
        {
            string teststring = "test";
            MemoryStream testStream = new MemoryStream(Encoding.UTF8.GetBytes(teststring));
            LoggingStream logging = new LoggingStream(testStream);
            logging.Length.Is(teststring.Length);
            StreamReader sr = new StreamReader(logging);
            sr.ReadToEnd().Is(teststring);
        }

        [TestMethod]
        public void LoggingStream_コールバック_Close()
        {
            string teststring = "test";
            long callback(long size, System.IO.Stream stream)
            {
                size.Is(teststring.Length);
                StreamReader csr = new StreamReader(stream);
                csr.ReadToEnd().Is(teststring);
                return size;
            }
            MemoryStream testStream = new MemoryStream(Encoding.UTF8.GetBytes(teststring));
            LoggingStream logging = new LoggingStream(testStream, callback);
            logging.Length.Is(teststring.Length);

            StreamReader sr = new StreamReader(logging);
            sr.ReadToEnd();
            logging.Close();
        }

        [TestMethod]
        public void LoggingStream_コールバック_Dispose()
        {
            string teststring = "test";
            long callback(long size, System.IO.Stream stream)
            {
                size.Is(teststring.Length);
                StreamReader csr = new StreamReader(stream);
                csr.ReadToEnd().Is(teststring);
                return size;
            }
            MemoryStream testStream = new MemoryStream(Encoding.UTF8.GetBytes(teststring));
            using (LoggingStream logging = new LoggingStream(testStream, callback))
            {
                logging.Length.Is(teststring.Length);
                StreamReader sr = new StreamReader(logging);
                sr.ReadToEnd();
            }
        }

        [TestMethod]
        public void LoggingStream_コールバック_SaveStreamなし()
        {
            string teststring = "test";
            long callback(long size, System.IO.Stream stream)
            {
                size.Is(teststring.Length);
                StreamReader csr = new StreamReader(stream);
                csr.ReadToEnd().Is("");
                return size;
            }
            MemoryStream testStream = new MemoryStream(Encoding.UTF8.GetBytes(teststring));
            using (LoggingStream logging = new LoggingStream(testStream, callback, false))
            {
                logging.Length.Is(teststring.Length);
                StreamReader sr = new StreamReader(logging);
                sr.ReadToEnd();
            }
        }
    }
}
