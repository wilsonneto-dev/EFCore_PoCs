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

        /* many to many */
        Actor a = new Actor() { Name = "Keano Reeves" };
        Actor a2 = new Actor() { Name = "Jenifer Lawrence" };

        Movie m1 = new Movie() { Name = "Matrix" };
        Movie m2 = new Movie() { Name = "Jhon Wick" };
        Movie m3 = new Movie() { Name = "Hunger Games" };
        m1.Actors = new List<Actor>() { a, a2 };
        m2.Actors = new List<Actor>() { a };

        db.Movies.Add(m1);
        db.Movies.Add(m2);
        db.Movies.Add(m3);
        db.Actors.Add(a2);
        db.SaveChanges();
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
    public List<Movie> Movies {get; set;}
}

public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Actor> Actors {get; set;}
}

public class AppContext : DbContext
{
    public DbSet<President> Presidents { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }

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
        // 1 - 1
        modelBuilder
            .Entity<Country>()
            .HasOne(x => x.President);
            
        modelBuilder
            .Entity<Country>()
            .Navigation(p => p.President).AutoInclude();

        /* 
        not necessary 1-N
        modelBuilder
            .Entity<Country>()
            .HasMany(x => x.Cities);
            .WithOne(x => x.Estado)
            .IsRequired(false) // podera haver cidade sem estado 
        */

        /*
        // .net default
        modelBuilder
            .Entity<Movie>()
            .HasMany(x => x.Actors)
            .WithMany(x => x.Movies)
            .UsingEntity(p => p.ToTable("ActorsMovies"));
        */

        // change the table fields (the default is the navigation property name + id name)
        /*
        modelBuilder
            .Entity<Movie>()
            .HasMany(x => x.Actors)
            .WithMany(x => x.Movies)
            .UsingEntity<Dictionary<string, object>>(
                "MoviesActors",
                p => p.HasOne<Movie>().WithMany().HasForeignKey("MovieIdentifier"),
                p => p.HasOne<Actor>().WithMany().HasForeignKey("ActorIdentifier"),
                p => {
                    p.Property<DateTime>("RegisteredAt").HasDefaulfValueSql("GETDATE()");
                }
            );
        */
    }
}
