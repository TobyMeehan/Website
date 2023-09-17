namespace TobyMeehan.Com.Accounts;

public class OAuth
{
    public class Parameters
    {
        public const string ResponseType = "response_type";
        public const string ClientId = "client_id";
        public const string CodeChallenge = "code_challenge";
        public const string CodeChallengeMethod = "code_challenge_method";
        public const string RedirectUri = "redirect_uri";
        public const string Scope = "scope";
        public const string State = "state";
        public const string GrantType = "grant_type";
        public const string Code = "code";
        public const string ClientSecret = "client_secret";
    }
    
    public class ResponseTypes
    {
        public const string Code = "code";
    }
    
    public class GrantTypes
    {
        public const string AuthorizationCode = "authorization_code";
    }
    
    public class Errors
    {
        public const string InvalidRequest = "invalid_request";
        public const string InvalidClient = "invalid_client";
        public const string InvalidGrant = "invalid_grant";
        public const string UnauthorizedClient = "unauthorized_client";
        public const string UnsupportedGrantType = "unsupported_grant_type";
        public const string InvalidScope = "invalid_scope";
    }
}