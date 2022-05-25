using Microsoft.IdentityModel.Tokens;

namespace AuthenticationEmbedder.Authentication;

public class SigningSymmetricKey : IJwtSigningDecodingKey, IJwtSigningEncodingKey
{
    public string SigningAlgorithm { get; } = SecurityAlgorithms.HmacSha256;
    public SymmetricSecurityKey SecretKey { private get; init; }

    public SecurityKey GetKey()
    {
        return SecretKey;
    }
}