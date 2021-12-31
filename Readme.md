Para criação do banco de dados com docker:
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Str0ngP455W0RD" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04

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

