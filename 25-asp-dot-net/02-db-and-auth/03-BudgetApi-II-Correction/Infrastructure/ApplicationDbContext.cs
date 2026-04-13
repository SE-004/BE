using Microsoft.EntityFrameworkCore;
using BudgetApi.Models;

namespace BudgetApi.Infrastructure;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

  public DbSet<Transaction> Transactions => Set<Transaction>();

}