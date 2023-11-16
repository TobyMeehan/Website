namespace TobyMeehan.Com.Models.User;

public interface ICreateUser
{
    string Username { get; }
    Password Password { get; }
}