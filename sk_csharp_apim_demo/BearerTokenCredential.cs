using Azure.Core;

#pragma warning disable IDE0009
public class BearerTokenCredential : TokenCredential
{
    private readonly AccessToken _accessToken;

    // Constructor that takes a Bearer token string and its expiration date
    public BearerTokenCredential(AccessToken accessToken)
    {
        this._accessToken = accessToken;
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return this._accessToken;
    }

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new ValueTask<AccessToken>(this._accessToken);
    }
}
