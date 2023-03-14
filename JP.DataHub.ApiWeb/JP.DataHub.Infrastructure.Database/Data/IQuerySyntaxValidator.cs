namespace JP.DataHub.Infrastructure.Database.Data
{
    public interface IQuerySyntaxValidator
    {
        bool Validate(string query, out string message);
    }
}
