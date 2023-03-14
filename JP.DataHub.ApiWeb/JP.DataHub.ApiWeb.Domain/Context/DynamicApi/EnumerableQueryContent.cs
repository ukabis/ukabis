using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Unity;
using Unity.Interception.Utilities;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    internal class EnumerableQueryContent : HttpContent
    {
        private IEnumerable<string> Content { get; }

        public EnumerableQueryContent(IEnumerable<string> content)
        : base()
        {
            Content = content;
        }

        public IEnumerable<string> GetContent()
        {
            return Content;
        }
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            StringBuilder sb = new StringBuilder();
            Content.ForEach(x => sb.Append(x));
            var st = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
            st.Seek(0, SeekOrigin.Begin);
            st.CopyTo(stream);
            var taskCompletionSource = new TaskCompletionSource<object>();
            taskCompletionSource.SetResult(stream);
            return taskCompletionSource.Task;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = Content.Sum(x => x.Length);
            return true;
        }
    }
}
