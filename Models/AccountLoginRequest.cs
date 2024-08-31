using System.ComponentModel.DataAnnotations;


/// <summary>
/// Represents a model for account login requests. 
/// This model is used when a user attempts to log in.
/// </summary>
public class AccountLoginRequestModel
{
    /// <summary>
    /// Gets or sets the username for the login request. 
    /// This is the unique identifier for each user.
    /// This property is required.
    /// </summary>
    [Required]
    public string username { get; set; }

    /// <summary>
    /// Gets or sets the password for the login request. 
    /// This property is required.
    /// </summary>
    [Required]
    public string password { get; set; }

    /// <summary>
    /// Gets or sets the expiry time for the login request.
    /// This could be used to ensure that the request is not older than a certain time, 
    /// or that the login session will not remain valid beyond this time.
    /// This property is required.
    /// </summary>
    [Required]
    public DateTime expire { get; set; }
}