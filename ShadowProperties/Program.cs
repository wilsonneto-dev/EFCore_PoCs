using System;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting");
        using var db = new AppContext();
        
        Position p = new() { Title = "Software Engineer", Sallary = 5000 };
        Employee Jorge = new() { Name = "Jorge", SurName = "B", CurrentPosition = p };
        
        db.Employees.Add(Jorge);

        // inserting the shadow property
        db.Entry(Jorge).Property("LastUpdate").CurrentValue = DateTime.Now;

        db.SaveChanges();

        // searching by a shadow property
        var result = db.Employees.Where(p => EF.Property<DateTime>(p, "LastUpdate") > DateTime.Now.AddHours(-1)).ToList();

        Console.WriteLine(result);
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
    public Position CurrentPosition { get; set; }
}

public class Position
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Sallary { get; set; }
}

public class AppContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }

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
            .UseSqlServer(@"Server=localhost;Database=company01;User Id=sa;Password=Str0ngP455W0RD");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // defining the shadow property
        modelBuilder.Entity<Employee>().Property<DateTime>("LastUpdate"); 
    }
}