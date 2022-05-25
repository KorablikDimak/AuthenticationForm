using System.Threading.Tasks;
using AuthenticationEmbedder.Models;
using InfoLog;

namespace AuthenticationEmbedder.Repository;

public interface IRepository
{
    public DataContext Context { get; init; }
    public ILogger Logger { get; set; }
        
    Task<bool> CreateEmailModel(EmailModel emailModel);
    Task<EmailModel> GetAuthModel(string token);
    Task<bool> DeleteAuthModel(string token);
    Task<bool> ValidateSiteLogin(SiteLogin siteLogin);
}