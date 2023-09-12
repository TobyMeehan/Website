namespace TobyMeehan.Com.Accounts.Models;

public class AuthorizeErrorModel
{
    public AuthorizeErrorModel(string error)
    {
        Error = error;
    }

    public AuthorizeErrorModel(string error, string? message)
    {
        Error = error;
        Message = message;
    }
    
    public string Error { get; set; }
    public string? Message { get; set; }
}