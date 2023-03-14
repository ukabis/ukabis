using System;

namespace JP.DataHub.Com.Json.Schema
{
    public interface ISchemaReference
    {
        Uri BaseUri { get; set; }
        Uri SubschemaId { get; set; }
    }
}