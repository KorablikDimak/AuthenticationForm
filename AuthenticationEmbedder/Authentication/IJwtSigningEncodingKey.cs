using Microsoft.IdentityModel.Tokens;

namespace AuthenticationEmbedder.Authentication;

public interface IJwtSigningEncodingKey
{
    string SigningAlgorithm { get; }
    SymmetricSecurityKey SecretKey { init; }
 
    SecurityKey GetKey();
}