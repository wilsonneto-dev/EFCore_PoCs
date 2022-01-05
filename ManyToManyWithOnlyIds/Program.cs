using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static void Main(string[] args)
    {
        using var db = new AppContext();
        var repo = new MovieRepository(db);
        
        
        Actor a = new Actor() { Name = "Keano Reeves", Id = Guid.NewGuid() };
        Actor a2 = new Actor() { Name = "Jenifer Lawrence", Id = Guid.NewGuid() };
        db.Actors.Add(a);
        db.Actors.Add(a2);
        db.SaveChanges(); 
        

        
        var movie01 = new Movie()
        {
            Id = Guid.NewGuid(),
            Name = "Just testing 02",
            ActorsIds = new List<Guid>() { new Guid("88237949-b13e-4793-bd5a-1eb01397bd94") , new Guid("c684c39e-22e6-410a-9291-98019510c98c")}
        };
        
        
        var movie = repo.Get(new Guid("3e35b3a6-ccd9-4fcd-aa0c-5dc95a3d43a3"));
        Console.WriteLine(movie.Name);
        Console.WriteLine(movie.ActorsIds.Count());

        movie.ActorsIds.Remove(movie.ActorsIds[1]);
        repo.Update(movie);

        var ma = new MovieActor() { 
            ActorId = new Guid("6575fe75-be17-4e7e-aa66-1ed9ae6e6b9b"), 
            MovieId = new Guid("b8762ed8-3b19-441c-a60c-88b913437b11") 
        };
        db.MovieActor.Add(ma);
        db.SaveChanges();

    }
}

public class Actor 
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // public List<Guid> MoviesIds { get; set; }
}

public class Movie
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Guid> ActorsIds { get; set; }
}


// infra object representing the relation
public class MovieActor
{
    public Guid MovieId { get; set; }
    public Movie Movie { get; set; }

    public Guid ActorId { get; set; }
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

    public void Insert(Movie movie)
    {
        _db.Movies.Add(movie);
        _db.MovieActor.AddRange(movie.ActorsIds.Select(x => new MovieActor(){ ActorId = x, MovieId = movie.Id }));
        _db.SaveChanges();
    }

    public void Update(Movie movie)
    {
        _db.Movies.Update(movie);
        _db.MovieActor.RemoveRange(_db.MovieActor.Where(x => x.MovieId == movie.Id));
        _db.MovieActor.AddRange(movie.ActorsIds.Select(x => new MovieActor(){ ActorId = x, MovieId = movie.Id }));
        _db.SaveChanges();
    }

    public Movie? Get(Guid id)
    {
        var movie = _db.Movies.FirstOrDefault(x => x.Id == id);
        if(movie == null) return null;
        movie.ActorsIds = _db.MovieActor.Where(x => x.MovieId == movie.Id).Select(x => x.ActorId).ToList();
        return movie;
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
        // Database.EnsureDeleted();
        // Database.EnsureCreated();
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
            .Ignore(x => x.ActorsIds);
        
        
        modelBuilder
            .Entity<MovieActor>()
            .HasKey(x => new{ x.ActorId, x.MovieId }); 
    }
}
