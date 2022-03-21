using System.ComponentModel.DataAnnotations;

namespace AuthenticationEmbedder.Models
{
    public class ResponseModel
    {
        [Required] public string Email { get; set; }
        [Required] public bool IsConfirmed { get; set; }
    }
}