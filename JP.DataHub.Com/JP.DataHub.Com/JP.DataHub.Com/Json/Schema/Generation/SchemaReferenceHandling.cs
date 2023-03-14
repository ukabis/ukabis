using System;

namespace JP.DataHub.Com.Json.Schema.Generation
{
    [Flags]
    public enum SchemaReferenceHandling
    {
        None = 0x0,
        Objects = 0x1,
        Arrays = 0x2,
        Dictionaries = 0x4,
        All = 0x7
    }
}