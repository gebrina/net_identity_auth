using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Net_Identity_Auth.Models;

namespace Net_Identity_Auth.DbContext;

public class AuthDbContext : IdentityDbContext<ApplicatoinUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}