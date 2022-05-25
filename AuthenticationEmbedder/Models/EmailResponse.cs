using System.ComponentModel.DataAnnotations;

namespace AuthenticationEmbedder.Models;

public class EmailResponse
{
    [Required] public string Email { get; set; }
    [Required] public bool IsConfirmed { get; set; }
}