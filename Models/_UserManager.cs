using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string UserName { get; set; }

    public string Email { get; set; }
}