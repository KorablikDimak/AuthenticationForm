using System.ComponentModel.DataAnnotations;

namespace AuthenticationEmbedder.Models
{
    public class LoginModel
    {
        public int Id { get; set; }
        [Required] public string SiteName { get; set; }
        [Required] public string Password { get; set; }
    }
}