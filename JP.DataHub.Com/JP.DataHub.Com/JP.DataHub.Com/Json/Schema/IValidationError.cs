using System;
using System.Collections.Generic;

namespace JP.DataHub.Com.Json.Schema
{
    public interface IValidationError
    {
        IList<IValidationError> ChildErrors { get; }
        ErrorType ErrorType { get; }
        int LineNumber { get; }
        int LinePosition { get; }
        string Message { get; }
        string Path { get; }
        IJSchema Schema { get; }
        Uri SchemaBaseUri { get; }
        Uri SchemaId { get; }
        object Value { get; }
    }
    public class ValidationError : IValidationError
    {
        public IList<IValidationError> ChildErrors => throw new NotImplementedException();

        public ErrorType ErrorType => throw new NotImplementedException();

        public int LineNumber => throw new NotImplementedException();

        public int LinePosition => throw new NotImplementedException();

        public string Message => throw new NotImplementedException();

        public string Path => throw new NotImplementedException();

        public IJSchema Schema => throw new NotImplementedException();

        public Uri SchemaBaseUri => throw new NotImplementedException();

        public Uri SchemaId => throw new NotImplementedException();

        public object Value => throw new NotImplementedException();
    }
}