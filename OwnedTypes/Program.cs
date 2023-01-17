using System;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static void Main(string[] args)
    {
        using var db = new AppContext();
        var employee = new Employee() { Id = Random.Shared.Next(10, 300), Address = null, Name = "Teste" };
        db.Add(employee);
        db.SaveChanges();

        // employee.Address = new Address() { District = "Bairro", Number = 30, Street = "Avenue St." };
        // db.Update(employee);
        // db.SaveChanges();

        var db2 = new AppContext();
        var employee2 = db2.Employees.AsNoTracking().Where(x => x.Id == employee.Id).First();
        employee2.Address = new Address(){ Number = 10, District = "Bairro" };
        db2.Update(employee2);
        db2.SaveChanges();
    }
}

public class Employee
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? SurName { get; set; }
    public Address? Address { get; set; }
}

public class Address
{
    public int Number { get; set; }
    public string? Street { get; set; }
    public string? District { get; set; }
}

public class AppContext : DbContext
{
    static bool Created = false;

    public DbSet<Employee> Employees { get; set; }

    public AppContext() : base()
    {
        if(Created == true)
            return;

        // Database.EnsureDeleted();
        Database.EnsureCreated();
        Created = true;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            //.UseSqlite("Data Source=./Application.db;");
            .UseSqlite("Data Source=Sharable;Mode=Memory;Cache=Shared");
            // .UseInMemoryDatabase("general");
            // .UseSqlServer(@"Server=localhost;Database=company02;User Id=sa;Password=Str0ngP455W0RD;trustServerCertificate=true;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(p => {
            p.OwnsOne( x => x.Address, a => {
                a.Property(p => p.District).HasColumnName("neighborhood");
                // a.ToTable("endereco");
            });
        });

        modelBuilder.Entity<Employee>().Property(x => x.Id).ValueGeneratedNever();
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