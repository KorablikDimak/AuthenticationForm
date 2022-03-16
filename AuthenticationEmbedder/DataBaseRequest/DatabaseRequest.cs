using System;
using System.Threading.Tasks;
using AuthenticationEmbedder.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationEmbedder.DataBaseRequest
{
    public class DatabaseRequest : IDatabaseRequest
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

        public async Task<bool> CheckSiteAsync(string siteName, string password)
        {
            try
            {
                LoginModel login = await Context.LoginModels.FirstOrDefaultAsync(data => 
                    data.SiteName == siteName && data.Password == password);
                if (login == null) return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}