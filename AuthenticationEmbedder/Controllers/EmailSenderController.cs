using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationEmbedder.Authentication;
using AuthenticationEmbedder.Models;
using AuthenticationEmbedder.Repository;
using InfoLog;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using Newtonsoft.Json;

namespace AuthenticationEmbedder.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmailSenderController : Controller
{
    private ILogger Logger { get; }
    private IRepository Repository { get;  }
    private IJwtSigningEncodingKey JwtSigningEncodingKey { get; }
        
    public EmailSenderController(ILogger logger, 
        IRepository repository, 
        IJwtSigningEncodingKey jwtSigningEncodingKey)
    {
        Logger = logger;
        Repository = repository;
        Repository.Logger = Logger;
        JwtSigningEncodingKey = jwtSigningEncodingKey;
    }

    [AllowAnonymous]
    [HttpPost("GetJwtToken")]
    public async Task<ActionResult<string>> GetJwtToken([FromBody] SiteLogin siteLogin)
    {
        if (!await Repository.ValidateSiteLogin(siteLogin)) return new ForbidResult();
            
        var claims = new Claim[]
        {
            new(ClaimTypes.NameIdentifier, siteLogin.SiteName)
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
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
    {
        EmailModel emailModel = CreateEmailModel(emailRequest);
        MimeMessage mimeMessage = await CreateMimeMessage(emailModel);
            
        if (!await Repository.CreateEmailModel(emailModel)) return new ConflictResult();
        if (!await SendMimeMessage(mimeMessage)) return new ConflictResult();

        return Ok();
    }

    private EmailModel CreateEmailModel(EmailRequest emailRequest)
    {
        var emailModel = new EmailModel
        {
            Email = emailRequest.EmailName,
            Token = Guid.NewGuid().ToString(),
            ResponseAddress = emailRequest.ResponseAddress,
            DateTime = DateTime.Now
        };

        return emailModel;
    }

    private async Task<MimeMessage> CreateMimeMessage(EmailModel emailModel)
    {
        var eMassage = new MimeMessage();
            
        eMassage.From.Add(new MailboxAddress("Confirm registration", "youremail"));
        eMassage.To.Add(new MailboxAddress("", emailModel.Email));
        string html = await System.IO.File.ReadAllTextAsync("Views/email body.html");
        html = html
            .Replace("useremailtoconfirm", $"{emailModel.Email}")
            .Replace("authenticationaddresswithtoken", 
                $"https://localhost:5001/api/EmailSender/ConfirmEmail?token={emailModel.Token}");
        eMassage.Body = new TextPart ("html") { Text = html };

        return eMassage;
    }

    private async Task<bool> SendMimeMessage(MimeMessage eMassage)
    {
        try
        {
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 465, true);
            await client.AuthenticateAsync("youremail", "tourpassword");
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

    [AllowAnonymous]
    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string token)
    {
        EmailModel emailModel = await Repository.GetAuthModel(token);
        if (emailModel == null) return new NotFoundResult();

        var responseModel = new EmailResponse
        {
            Email = emailModel.Email,
            IsConfirmed = false
        };
        if (!(DateTime.Now.Subtract(emailModel.DateTime) > TimeSpan.FromMinutes(15))) responseModel.IsConfirmed = true;
        if (!await Repository.DeleteAuthModel(emailModel.Token)) return new ConflictResult();

        using (var client = new HttpClient())
        {
            try
            {
                var json = JsonConvert.SerializeObject(responseModel);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, emailModel.ResponseAddress)
                    { Content = content };
                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new ConflictResult();
            }
            catch (Exception e)
            {
                await Logger.Warning(e.ToString());
                return new ConflictResult();
            }
        }

        return Redirect(emailModel.ResponseAddress);
    }
}