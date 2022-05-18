using System.Threading.Tasks;
using AuthenticationEmbedder.Models;

namespace AuthenticationEmbedder.DataBaseRequest
{
    public interface IRepository
    {
        public DataContext Context { get; init; }
        
        Task<bool> AddAuthModelAsync(AuthModel authModel);
        Task<AuthModel> FindAuthModelAsync(string token);
        Task<bool> DeleteAuthModelAsync(string token);
        Task<bool> CheckSiteAsync(SiteLogin siteLogin);
    }
}