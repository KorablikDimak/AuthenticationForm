using Microsoft.IdentityModel.Tokens;

namespace AuthenticationEmbedder.Authentication;

public interface IJwtSigningDecodingKey
{
    SecurityKey GetKey();
}