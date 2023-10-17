using Common.Enum;
using Common.Models;

namespace Interfaces;

public interface IQueryPluginService
{
    Task<QueryResponse> GetStoredData(Query query, int queryId);
}
