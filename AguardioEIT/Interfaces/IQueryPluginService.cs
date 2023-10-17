using System.Threading.Tasks;
using Common.Enum;
using Common.Models;

namespace Interfaces;

public interface IQueryPluginService
{
    Task<QueryResponse> GetStoredData(string cacheKey, Query query, int queryId);
}
