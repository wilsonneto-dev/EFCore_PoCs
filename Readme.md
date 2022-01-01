Para criação do banco de dados com docker:
`docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Str0ngP455W0RD" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04`

### Shadow Properties

```
        // defining the shadow property
        modelBuilder.Entity<Employee>().Property<DateTime>("LastUpdate"); 

        // inserting the shadow property
        db.Entry(Jorge).Property("LastUpdate").CurrentValue = DateTime.Now;

        // searching by a shadow property
        var result = db.Employees.Where(p => EF.Property<DateTime>(p, "LastUpdate") > DateTime.Now.AddHours(-1)).ToList();
```


### Owned Types

```
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

```

```
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
```

## Relations

#### Lazy Loading

```
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()
        [...]
    
    var country = db.Countries.SingleOrDefault(x => x.Id == 1);
    Console.WriteLine(country.Name);
    Console.WriteLine(country.PresidentId);

    // reference will be loaded here
    Console.WriteLine(country.President.Name);
```

#### Eagger Loading

```
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLazyLoadingProxies()
        [...]
    
        // already loaded here
        var country = db.Countries
            .Include(c => c.President)
            .SingleOrDefault(x => x.Id == 1);

        Console.WriteLine(country.President.Name);

```

#### Explicity loading

```
Explicit Loading:

var country = db.Countries.SingleOrDefault(x => x.Id == 1);
Console.WriteLine(country.Name);
Console.WriteLine(country.PresidentId);

db.Entry(country).Reference(c => c.President).Load(); 
Console.WriteLine(country.President.Name);
```

#### Auto Iclude properties

```
    modelBuilder
        .Entity<Country>()
        .HasOne(x => x.President);
    
    // here auto included property
    modelBuilder
        .Entity<Country>()
        .Navigation(p => p.President).AutoInclude();
    [...]

    var country = db.Countries.Find(1);
    Console.WriteLine(country.President.Name);
```

#### Relations

```
        modelBuilder
            .Entity<Country>()
            .HasOne(x => x.President);
            
        modelBuilder
            .Entity<Country>()
            .Navigation(p => p.President).AutoInclude();

        not necessary 1-N
        modelBuilder
            .Entity<Country>()
            .HasMany(x => x.Cities);
            .WithOne(x => x.Estado)
            .IsRequired(false) // podera haver cidade sem estado 

        modelBuilder
            .Entity<Movie>()
            .HasMany(x => x.Actors)
            .WithMany(x => x.Movies)
            .UsingEntity(p => p.ToTable("ActorsMovies"));
```
