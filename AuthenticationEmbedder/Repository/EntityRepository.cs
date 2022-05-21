using System;
using System.Threading.Tasks;
using AuthenticationEmbedder.Models;
using InfoLog;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationEmbedder.Repository
{
    public class EntityRepository : IRepository
    {
        public ILogger Logger { get; set; }
        public DataContext Context { get; init; }

        public async Task<bool> AddAuthModelAsync(AuthModel authModel)
        {
            try
            {
                AuthModel auth = await FindAuthModelAsync(authModel.Token);
                if (auth != null) return false;

                await Context.AuthModels.AddAsync(authModel);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await Logger?.Error(e.ToString())!;
                return false;
            }

            return true;
        }

        public async Task<AuthModel> FindAuthModelAsync(string token)
        {
            AuthModel authModel = await Context.AuthModels.FirstOrDefaultAsync(data => data.Token == token);
            return authModel;
        }

        public async Task<bool> DeleteAuthModelAsync(string token)
        {
            try
            {
                AuthModel authModel = await FindAuthModelAsync(token);
                if (authModel == null) return false;
            
                Context.AuthModels.Remove(authModel);
                await Context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await Logger?.Error(e.ToString())!;
                return false;
            }

            return true;
        }

        public async Task<bool> CheckSiteAsync(SiteLogin siteLogin)
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
}