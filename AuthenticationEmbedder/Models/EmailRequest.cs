using System.ComponentModel.DataAnnotations;

namespace AuthenticationEmbedder.Models;

public class EmailRequest
{
    [Required] public string EmailName { get; set; }
    [Required] public string ResponseAddress { get; set; }
}