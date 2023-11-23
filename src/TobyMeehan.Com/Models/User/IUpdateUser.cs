namespace TobyMeehan.Com.Models.User;

public interface IUpdateUser
{
    Optional<string> Username { get; }
    Optional<Password> Password { get; }
    Optional<string> DisplayName { get; }
    Optional<string?> Description { get; }
    Optional<Id<IAvatar>?> Avatar { get; }
}