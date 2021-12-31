using System;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static void Main(string[] args)
    {
        using var db = new AppContext();
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string SurName { get; set; }
    public Address Address { get; set; }
}

public class Address
{
    public int Number { get; set; }
    public string Street { get; set; }
    public string District { get; set; }
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
            .UseSqlServer(@"Server=localhost;Database=company02;User Id=sa;Password=Str0ngP455W0RD");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(p => {
            p.OwnsOne( x => x.Address, a => {
                a.Property(p => p.District).HasColumnName("neighborhood");
                a.ToTable("endereco");
            });
        });
    }
}

/*
    p.OwnsOne( x => x.Address ):

    CREATE TABLE [Employees] (
          [Id] int NOT NULL IDENTITY,
          [Name] nvarchar(max) NOT NULL,
          [SurName] nvarchar(max) NOT NULL,
          [Address_Number] int NOT NULL,
          [Address_Street] nvarchar(max) NOT NULL,
          [Address_District] nvarchar(max) NOT NULL,
          CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
      );

    ------------------------------------------
    p.OwnsOne( x => x.Address ).ToTable("endereco"):

    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [SurName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
    );
    CREATE TABLE [endereco] (
        [EmployeeId] int NOT NULL,
        [Number] int NOT NULL,
        [Street] nvarchar(max) NOT NULL,
        [District] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_endereco] PRIMARY KEY ([EmployeeId]),
        CONSTRAINT [FK_endereco_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
    );

    -----------------------------------------
    modelBuilder.Entity<Employee>(p => {
        p.OwnsOne( x => x.Address, a => {
            a.Property(p => p.District).HasColumnName("neighborhood");
        });
    });

    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [SurName] nvarchar(max) NOT NULL,
        [Address_Number] int NOT NULL,
        [Address_Street] nvarchar(max) NOT NULL,
        [neighborhood] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
    );

    ------------------------------------------------
    modelBuilder.Entity<Employee>(p => {
        p.OwnsOne( x => x.Address, a => {
            a.Property(p => p.District).HasColumnName("neighborhood");
            a.ToTable("endereco");
        });
    });

    CREATE TABLE [Employees] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [SurName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Employees] PRIMARY KEY ([Id])
    );

    CREATE TABLE [endereco] (
        [EmployeeId] int NOT NULL,
        [Number] int NOT NULL,
        [Street] nvarchar(max) NOT NULL,
        [neighborhood] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_endereco] PRIMARY KEY ([EmployeeId]),
        CONSTRAINT [FK_endereco_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE CASCADE
    );
*/