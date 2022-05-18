using System;
using System.Threading.Tasks;
using AuthenticationEmbedder.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationEmbedder.DataBaseRequest
{
    public class DataRequest : IRepository
    {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}