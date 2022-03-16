using System.Threading.Tasks;
using AuthenticationEmbedder.Models;

namespace AuthenticationEmbedder.DataBaseRequest
{
    public interface IDatabaseRequest
    {
        public DataContext Context { get; init; }
        
        Task<bool> AddAuthModelAsync(AuthModel authModel);
        Task<AuthModel> FindAuthModelAsync(string token);
        Task<bool> DeleteAuthModelAsync(string token);
        Task<bool> CheckSiteAsync(string siteName, string password);
    }
}