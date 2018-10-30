using System.Data;

namespace KRF.Common
{
    public interface IDataAccessFactory
    {
        string ConnectionString { get; }

        IDbConnection CreateConnection();
    }
}