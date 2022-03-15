namespace AuthenticationEmbedder.Models
{
    public class AuthModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string ResponseAddress { get; set; }
        public bool IsConfirmed { get; set; }
    }
}