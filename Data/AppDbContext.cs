using Microsoft.EntityFrameworkCore;
using MovieClone.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Movie> Watchlist => Set<Movie>();
}