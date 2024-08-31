/// <summary>
/// Represents a model for account login responses.
/// This model is used when the server is responding to a client's login request.
/// It inherits from the ResponseBaseModel class.
/// </summary>
public class AccountLoginResponseModel
{
    /// <summary>
    /// Gets or sets the authentication token for the user.
    /// This token is usually used for subsequent requests to the server to verify the user's identity and permissions.
    /// /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Gets or sets the expiration time for the authentication token.
    /// This indicates the time until which the token is valid. After this time, the user may need to log in again.
    /// </summary>
    public DateTime Expire { get; set; }
}