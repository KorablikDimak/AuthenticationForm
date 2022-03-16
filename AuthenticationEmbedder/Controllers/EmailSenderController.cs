using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthenticationEmbedder.Authentication;
using AuthenticationEmbedder.DataBaseRequest;
using AuthenticationEmbedder.Models;
using InfoLog;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

namespace AuthenticationEmbedder.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailSenderController : Controller
    {
        private ILogger Logger { get; }
        private IDatabaseRequest DatabaseRequest { get;  }
        private IJwtSigningEncodingKey JwtSigningEncodingKey { get; }
        
        public EmailSenderController(ILogger logger, 
            IDatabaseRequest databaseRequest, 
            IJwtSigningEncodingKey jwtSigningEncodingKey)
        {
            JwtSigningEncodingKey = jwtSigningEncodingKey;
            DatabaseRequest = databaseRequest.CastToDatabaseRequestWithLogger(logger);
            Logger = logger;
        }

        [HttpGet("GetJwtToken")]
        public async Task<ActionResult<string>> GetJwtToken(
            [FromHeader(Name = "site")] string siteName,
            [FromHeader(Name = "password")] string password)
        {
            if (!await DatabaseRequest.CheckSiteAsync(siteName, password)) return new ForbidResult();
            
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, siteName)
            };
 
            var token = new JwtSecurityToken(
                issuer: "AuthenticationEmbedder",
                audience: "AuthenticationEmbedderClient",
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: new SigningCredentials(
                    JwtSigningEncodingKey.GetKey(),
                    JwtSigningEncodingKey.SigningAlgorithm)
            );
 
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            return jwtToken;
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail(
            [FromHeader(Name = "email")] string email, 
            [FromHeader(Name = "responseaddress")] string responseAddress)
        {
            AuthModel authModel = new AuthModel
            {
                Email = email,
                Token = Guid.NewGuid().ToString(),
                ResponseAddress = responseAddress,
                IsConfirmed = false
            };
            
            if (!await DatabaseRequest.AddAuthModelAsync(authModel)) return new ConflictResult();
            if (!await SendEmailAsync(authModel)) return new ConflictResult();
            return Ok();
        }

        private async Task<bool> SendEmailAsync(AuthModel authModel)
        {
            var eMassage = new MimeMessage();
            
            eMassage.From.Add(new MailboxAddress("Confirm registration", "authenticationemailsender@gmail.com"));
            eMassage.To.Add(new MailboxAddress("", authModel.Email));
            string html = await System.IO.File.ReadAllTextAsync("Views/email body.html");
            html = html
                .Replace("useremailtoconfirm", $"{authModel.Email}")
                .Replace("authenticationaddresswithtoken", 
                    $"https://localhost:5001/api/EmailSender/ConfirmEmail?token={authModel.Token}");
            eMassage.Body = new TextPart ("html") { Text = html };

            try
            {
                using var client = new SmtpClient();
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("authenticationemailsender@gmail.com", "*****");
                await client.SendAsync(eMassage);
                await client.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                await Logger.Warning(e.ToString());
                return false;
            }
            
            return true;
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            AuthModel authModel = await DatabaseRequest.FindAuthModelAsync(token);
            if (authModel == null) return new NotFoundResult();
            authModel.IsConfirmed = true;

            using (var client = new HttpClient())
            {
                try
                {
                    await DatabaseRequest.DeleteAuthModelAsync(authModel.Token);
                    await client.PostAsJsonAsync(new Uri(authModel.ResponseAddress), authModel);
                }
                catch (Exception e)
                {
                    await Logger.Warning(e.ToString());
                    return new ConflictResult();
                }
            }

            return Redirect(authModel.ResponseAddress);
        }
    }
}