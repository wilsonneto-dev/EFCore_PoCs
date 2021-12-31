using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static void Main(string[] args)
    {
        using var db = new AppContext();

        var c = new Country() { 
            Name = "US", 
            President = new President() { Name = "Obama"},
            Cities = new List<City>()
        };
        
        City city1 = new City() { Name = "Washington" };
        City city2 = new City() { Name = "NY" };

        c.Cities.Add(city1);
        c.Cities.Add(city2);

        db.Countries.Add(c);
        db.SaveChanges();

        var country = db.Countries.Include(x => x.Cities).SingleOrDefault(x => x.Id == 1);

        Console.WriteLine(country.Name);
        Console.WriteLine(country.President.Name);
        Console.WriteLine(country.Cities[0].Name);
    }
}

public class President
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Country Country { get; set; }
}

public class Country
{
    public int Id {get; set; }
    public string Name { get; set; }
    public President President { get; set; }    
    public int PresidentId { get; set; }
    public List<City> Cities { get; set; }
}

public class City 
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Actor 
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class AppContext : DbContext
{
    public DbSet<President> Presidents { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }

    public AppContext() : base()
    {
        Database.EnsureDeleted();
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseSqlServer(@"Server=localhost;Database=company02;User Id=sa;Password=Str0ngP455W0RD");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Country>()
            .HasOne(x => x.President);
            
        modelBuilder
            .Entity<Country>()
            .Navigation(p => p.President).AutoInclude();

        /* 
        modelBuilder
            .Entity<Country>()
            .HasMany(x => x.Cities);
            .WithOne(x => x.Estado)
            .IsRequired(false) // podera haver cidade sem estado 
        */

    }
}
