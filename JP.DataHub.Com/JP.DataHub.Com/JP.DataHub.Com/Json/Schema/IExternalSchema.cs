using System;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IExternalSchema
    {
        IJSchema Schema { get; }
        Uri Uri { get; }
    }
}