namespace Integration_with_external_party.Infrastructure;

using Integration_with_external_party.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    public DbSet<Employee> Employees { get; set; }
}