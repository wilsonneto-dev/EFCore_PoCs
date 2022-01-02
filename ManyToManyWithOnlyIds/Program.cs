using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static void Main(string[] args)
    {
        using var db = new AppContext();

        /* 
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
        */

        /* 
        var m = db.Movies.FirstOrDefault(x => x.Id == 1);
        Console.WriteLine(m.Name);
        Console.WriteLine(m.Actors.Count());
        Console.WriteLine(m.ActorsIds.Count());
        */    
    }
}

// domain entity
public class Actor 
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// domain entity
public class Movie
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<int> ActorIds { get; set; }

}

// infra object representing the relation
public class MovieActor
{
    public int MovieId { get; set; }
    public Movie Movie { get; set; }

    public int ActorId { get; set; }
    public Actor Actor { get; set; }
}

// Movie repository
public class MovieRepository
{
    private readonly AppContext _db;

    public MovieRepository(AppContext db)
    {
        _db = db;
    }

    
}

// db context
public class AppContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }

    // mapping relational table
    public DbSet<MovieActor> MovieActor { get; set; }

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
            .Entity<Movie>()
            .Ignore(x => x.ActorIds);
        
        modelBuilder
            .Entity<MovieActor>()
            .HasKey(x => new { x.MovieId, x.ActorId });
    }
}
