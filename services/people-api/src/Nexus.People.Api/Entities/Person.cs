using Nexus.Common;

namespace Nexus.PeopleAPI.Entities;

public class Person : AuditableNexusEntityBase
{
    public Person(string name, string email, string password)
    {
        Name = name;
        Email = email;
        Password = password;
    }

    public Person(string name, string email)
    {
        Name = name;
        Email = email;
        Password = string.Empty;
    }
    
    public string Name { get; private set; }

    public string Email { get; private set; }

    public string IdentityId { get; set; } = string.Empty;

    public string Password { get; private set; }

    public void UpdateName(string newName)
    {
        Name = newName;
    }

    public void UpdateEmail(string newEmail)
    {
        Email = newEmail;
    }
}
