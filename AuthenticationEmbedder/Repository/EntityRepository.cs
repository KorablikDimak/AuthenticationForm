using System;
using System.Threading.Tasks;
using AuthenticationEmbedder.Models;
using InfoLog;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationEmbedder.Repository;

public class EntityRepository : IRepository
{
    public ILogger Logger { get; set; }
    public DataContext Context { get; init; }

    public async Task<bool> CreateEmailModel(EmailModel emailModel)
    {
        try
        {
            EmailModel email = await GetAuthModel(emailModel.Token);
            if (email != null) return false;

            await Context.EmailModels.AddAsync(emailModel);
            await Context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            await Logger?.Error(e.ToString())!;
            return false;
        }

        return true;
    }

    public async Task<EmailModel> GetAuthModel(string token)
    {
        EmailModel emailModel = await Context.EmailModels.FirstOrDefaultAsync(data => data.Token == token);
        return emailModel;
    }

    public async Task<bool> DeleteAuthModel(string token)
    {
        try
        {
            EmailModel emailModel = await GetAuthModel(token);
            if (emailModel == null) return false;
            
            Context.EmailModels.Remove(emailModel);
            await Context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            await Logger?.Error(e.ToString())!;
            return false;
        }

        return true;
    }

    public async Task<bool> ValidateSiteLogin(SiteLogin siteLogin)
    {
        try
        {
            SiteLogin siteLoginResult = await Context.LoginModels.FirstOrDefaultAsync(data => 
                data.SiteName == siteLogin.SiteName && data.Password == siteLogin.Password);
            if (siteLoginResult == null) return false;
        }
        catch (Exception e)
        {
            await Logger?.Error(e.ToString())!;
            return false;
        }

        return true;
    }
}