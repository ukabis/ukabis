using System;

namespace JP.DataHub.Com.Json.Schema
{
    public enum JSchemaType
    {
        None = 0x0,
        String = 0x1,
        Number = 0x2,
        Integer = 0x4,
        Boolean = 0x8,
        Object = 0x10,
        Array = 0x20,
        Null = 0x40
    }
}
