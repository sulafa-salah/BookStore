using ErrorOr;
using Identity.Domain.Common;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Domain.UserAggregate;
public class ApplicationUser : IdentityUser<Guid>, IAggregateRoot
{

  
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public bool IsActive { get; private set; } = true;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    // Constructors
    protected ApplicationUser() { }

    private ApplicationUser(Guid id, string firstName, string lastName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        UserName = email;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;

    }

  
    public static ErrorOr<ApplicationUser> Create(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return UserErrors.InvalidFirstName;

        if (string.IsNullOrWhiteSpace(lastName))
            return UserErrors.InvalidLastName;

        if (string.IsNullOrWhiteSpace(email))
            return UserErrors.InvalidEmail;

        if (!email.Contains("@"))
            return UserErrors.InvalidEmailFormat;

        var user = new ApplicationUser(Guid.NewGuid(), firstName.Trim(), lastName.Trim(), email.Trim());
        return user;
    }

    // Domain events
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public List<IDomainEvent> PopDomainEvents()
    {
        var events = _domainEvents.ToList();
        _domainEvents.Clear();
        return events;
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}