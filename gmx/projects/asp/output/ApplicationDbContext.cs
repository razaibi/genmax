using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SampleWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace SampleWebApp.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        this.SeedRoles(builder);
        this.SeedUsers(builder);
        this.SeedUserRoles(builder);
    }

    private void SeedUsers(ModelBuilder builder)
    {
        PasswordHasher<IdentityUser> passwordHasher = new PasswordHasher<IdentityUser>();
        IdentityUser user = new IdentityUser()
        {
            Id = "b74ddd14-6340-4840-95c2-db12554843e5",
            UserName = "admin@",
            NormalizedUserName = "ADMIN@SAMPLE.COM",
            Email = "admin@sample.com",
            NormalizedEmail = "ADMIN@SAMPLE.COM",
            LockoutEnabled = false.
            SecurityStamp = new Guid().ToString("D"),
            EmailConfirmed = true,
            TwoFactorEnabled = false
        };

        user.PasswordHash = passwordHasher.HashPassword(user, "Admunto_123#");
        builder.Entity<IdentityUser>().HasData(user);
    }

    private void SeedRoles(ModelBuilder builder)
    {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole() {
                    Id = "fab4fac1-c546-41de-aebc-a14da6895711",
                    Name = "Adminstrator",
                    ConcurrencyStamp = "1",
                    NormalizedName = "ADMINISTRATOR"
                }
            );
    }

    private void SeedUserRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityUserRole<string>().HasData(
            new IdentityUserRole<string>(){
                RoleId = "fab4fac1-c546-41de-aebc-a14da6895711",
                UserId = "b74ddd14-6340-4840-95c2-db12554843e5"
            }
        );
    }
    public DbSet<SampleWebApp.Models.ProgramStatus> ProgramStatus { get;set; }
    public DbSet<SampleWebApp.Models.Organization> Organization { get;set; }
    public DbSet<SampleWebApp.Models.Member> Member { get;set; }
    public DbSet<SampleWebApp.Models.MemberGroup> MemberGroup { get;set; }
    public DbSet<SampleWebApp.Models.MemberGroupMapping> MemberGroupMapping { get;set; }
    public DbSet<SampleWebApp.Models.MemberGroupPermission> MemberGroupPermission { get;set; }
    public DbSet<SampleWebApp.Models.Note> Note { get;set; }
    public DbSet<SampleWebApp.Models.MemberPermission> MemberPermission { get;set; }
    public DbSet<SampleWebApp.Models.Sample> Sample { get;set; }
}