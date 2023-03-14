using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record OutputStream : IValueObject
    {
        public Stream Value { get; }

        public OutputStream(Stream value)
        {
            this.Value = value;
        }
    }
}
