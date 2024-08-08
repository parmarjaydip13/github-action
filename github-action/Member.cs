namespace github_action;

public sealed class Member
{

    private Member(Guid id, string email, string firstName, string lastName)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
    }

    private Member()
    {

    }

    public Guid Id { get; private init; }

    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public DateTime CreatedOnUtc { get; set; }

    public DateTime? ModifiedOnUtc { get; set; }

    public static Member Create(
        Guid id,
        string email,
        string firstName,
        string lastName)
    {
        var member = new Member(
            id,
            email,
            firstName,
            lastName);
        return member;
    }
}

