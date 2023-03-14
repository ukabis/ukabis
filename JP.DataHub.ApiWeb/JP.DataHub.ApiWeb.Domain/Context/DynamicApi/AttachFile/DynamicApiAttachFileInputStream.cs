using System.Net.Http.Headers;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile
{
    // .NET6
    internal record DynamicApiAttachFileInputStream : IValueObject
    {
        /// <summary>
        /// 入力ストリーム
        /// </summary>
        public Stream InputStream { get; }
        /// <summary>
        /// 最初のストリームか
        /// </summary>        
        public bool IsStartStream { get; }
        /// <summary>
        /// 最後のストリームか
        /// </summary>        
        public bool IsEndStream { get; }
        /// <summary>
        /// 追加のストリームか
        /// </summary>
        public bool IsAppendStream { get; }
        /// <summary>
        /// ストリーム追加場所
        /// </summary>
        public long AppendPosition { get; }

        public DynamicApiAttachFileInputStream(Stream inputStream, ContentRangeHeaderValue range = null)
        {
            InputStream = inputStream;

            IsStartStream = (range == null) || (range.From == 0);
            IsEndStream = (range == null) || (range.To == range.Length - 1);
            long aPosition = 0;
            if (range != null)
            {
                aPosition = range.From ?? 0;
            }
            AppendPosition = aPosition;
            IsAppendStream = (range != null) && (range.From != 0);
        }

        public static bool operator ==(DynamicApiAttachFileInputStream me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DynamicApiAttachFileInputStream me, object other) => !me?.Equals(other) == true;
    }
}