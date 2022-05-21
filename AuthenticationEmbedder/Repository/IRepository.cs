using System.Threading.Tasks;
using AuthenticationEmbedder.Models;
using InfoLog;

namespace AuthenticationEmbedder.Repository
{
    public interface IRepository
    {
        public DataContext Context { get; init; }
        public ILogger Logger { get; set; }
        
        Task<bool> AddAuthModelAsync(AuthModel authModel);
        Task<AuthModel> FindAuthModelAsync(string token);
        Task<bool> DeleteAuthModelAsync(string token);
        Task<bool> CheckSiteAsync(SiteLogin siteLogin);
    }
}