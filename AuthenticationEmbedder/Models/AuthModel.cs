using System;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationEmbedder.Models
{
    public class AuthModel
    {
        public int Id { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Token { get; set; }
        [Required] public string ResponseAddress { get; set; }
        [Required] public DateTime DateTime { get; set; }
    }
}