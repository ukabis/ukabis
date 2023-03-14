namespace JP.DataHub.Infrastructure.Database.Data
{
    public interface IQuerySyntaxValidatorFactory
    {
        IQuerySyntaxValidator Create(string query);
    }
}
