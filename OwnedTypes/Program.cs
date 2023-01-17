using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class Program
{
    static void Main(string[] args)
    {
        var id = Guid.Empty;
        using (var db = new AppContext())
        {
            var video = new Video("Test", "ooook");
            // video.UpdateMedia("old", "old");
            // video.UpdateTrailer("old", "old");
            // video.UpdateThumb("thumb-path");
            // video.UpdateBanner("banner-path");

            id = video.Id;
            db.Add(video);
            Console.WriteLine(JsonSerializer.Serialize(video, new JsonSerializerOptions() { WriteIndented = true }));
            // Console.WriteLine(JsonSerializer.Serialize(new{
            //     video = db.Entry(video).State,
            //     media = db.Entry(video.Media).State,
            //     trailer = db.Entry(video.Trailer).State,
            // }));
            db.SaveChanges();
        }

        Console.WriteLine(" --- updating...");
        var newDbContext = new AppContext();
        var video2 = newDbContext.Videos.AsNoTracking().Where(x => x.Id == id).First();

        video2.UpdateThumb("new:thumb-path");
        video2.UpdateBanner("new:banner-path");
        video2.UpdateMedia("new:media-path", "new:encoded-media-path");
        video2.UpdateTrailer("new:trailer-path", "new:encoded-trailer-path");

        Console.WriteLine(JsonSerializer.Serialize(video2, new JsonSerializerOptions() { WriteIndented = true }));

        // newDbContext.Add(video2);
        newDbContext.Videos.Update(video2);

        // Console.WriteLine(JsonSerializer.Serialize(new{
        //     video = newDbContext.Entry(video2).State,
        //     media = newDbContext.Entry(video2.Media).State,
        //     trailer = newDbContext.Entry(video2.Trailer).State,
        // }));

        newDbContext.SaveChanges();
    }
}

public class AppContext : DbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Video>(p => {
            p.Property(x => x.Id).ValueGeneratedNever();
            p.Navigation(x => x.Media).AutoInclude();
            p.Navigation(x => x.Trailer).AutoInclude();

            p.OwnsOne( x => x.Thumb, a => {
                a.Property(p => p.Path).IsRequired(false).HasColumnName("ThumbPath");
            });

            p.OwnsOne( x => x.Banner, a => {
                a.Property(p => p.Path).IsRequired(false).HasColumnName("BannerPath");
            });
            
            p.HasOne(x => x.Media).WithOne().HasForeignKey<Video>("MediaId");
            p.HasOne(x => x.Trailer).WithOne().HasForeignKey<Video>("TrailerId");
            // p.HasOne(x => x.Media).WithOne("VideoId").HasForeignKey<Media>();
        });

        modelBuilder.Entity<Media>(p => {
            // p.HasKey(x => x.Id);
            p.Property(x => x.Id).ValueGeneratedNever();
        });
    }

    static bool Created = false;

    public DbSet<Video> Videos => Set<Video>();

    public AppContext() : base()
    {
        if(Created == true)
            return;

        Database.EnsureDeleted();
        Database.EnsureCreated();
        Created = true;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseSqlServer(@"Server=localhost;Database=company02;User Id=sa;Password=Str0ngP455W0RD;trustServerCertificate=true;");
            //.UseSqlite("Data Source=./Application.db;");
            // .UseSqlite("Data Source=Sharable;Mode=Memory;Cache=Shared");
            // .UseInMemoryDatabase("general");
    }
}

public class Video
{
    public Video(string title, string description)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    
    public Media? Media { get; private set; }
    public Media? Trailer { get; private set; }
    public Image? Thumb { get; private set; }
    public Image? Banner { get; private set; }

    public void UpdateThumb(string path) => Thumb = new Image(path);
    
    public void UpdateBanner(string path) => Banner = new Image(path);

    public void UpdateMedia(string path, string encodedPath)
    {
        if(Media is null)
            Media = new Media(path, encodedPath);
        else
            Media.Update(path, encodedPath);
    }

    public void UpdateTrailer(string path, string encodedPath)
    {
        if(Trailer is null)
            Trailer = new Media(path, encodedPath);
        else
            Trailer.Update(path, encodedPath);
    }
}

public class Image
{
    public Image(string path) => Path = path;

    public string Path { get; init; }
}

public class Media
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; private set; }
    public string Path { get; private set; }
    public string EncodedPath { get; private set; }

    public Media(string path, string encodedPath)
    {
        Id = Guid.NewGuid();
        Path = path;
        EncodedPath = encodedPath;
    }

    public void Update(string path, string encodedPath)
    {
        Path = path;
        EncodedPath = encodedPath;
    }
}