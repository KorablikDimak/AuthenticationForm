using System.Threading.Tasks;
using AuthenticationEmbedder.Models;
using InfoLog;

namespace AuthenticationEmbedder.Repository;

public interface IRepository
{
    public DataContext Context { get; init; }
    public ILogger Logger { get; set; }
        
    Task<bool> CreateEmailModelAsync(EmailModel emailModel);
    Task<EmailModel> GetEmailModelAsync(string token);
    Task<bool> DeleteEmailModelAsync(string token);
    Task<bool> ValidateSiteLoginAsync(SiteLogin siteLogin);
}