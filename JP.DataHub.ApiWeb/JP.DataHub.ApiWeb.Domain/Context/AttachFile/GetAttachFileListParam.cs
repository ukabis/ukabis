using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal class GetAttachFileListParam : IValueObject
    {
        public enum SortIndexEnum
        {
            FileName,
            RegisterDateTime,
            RegisterUserId
        }

        public enum SortOrderEnum
        {
            Asc,
            Desc
        }

        public SortIndexEnum? SortIndex { get; }
        public SortOrderEnum SortOrder { get; }


        public GetAttachFileListParam(SortIndexEnum? sortIndex, SortOrderEnum sortOrder)
        {
            SortIndex = sortIndex;
            SortOrder = sortOrder;
        }
    }
}
