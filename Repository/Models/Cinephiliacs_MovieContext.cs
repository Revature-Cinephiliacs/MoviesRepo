using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repository.Models
{
    public partial class Cinephiliacs_MovieContext : DbContext
    {
        public Cinephiliacs_MovieContext()
        {
        }

        public Cinephiliacs_MovieContext(DbContextOptions<Cinephiliacs_MovieContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Actor> Actors { get; set; }
        public virtual DbSet<Director> Directors { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<MovieActor> MovieActors { get; set; }
        public virtual DbSet<MovieDirector> MovieDirectors { get; set; }
        public virtual DbSet<MovieGenre> MovieGenres { get; set; }
        public virtual DbSet<MovieLanguage> MovieLanguages { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:cinephiliacs.database.windows.net,1433;Initial Catalog=Cinephiliacs_Movie;Persist Security Info=False;User ID=kugelsicher;Password=F36UWevqvcDxEmt;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Actor>(entity =>
            {
                entity.ToTable("Actor");

                entity.HasIndex(e => e.ActorName, "uk_actorName")
                    .IsUnique();

                entity.Property(e => e.ActorId)
                    .HasColumnName("actorId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ActorName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("actorName");
            });

            modelBuilder.Entity<Director>(entity =>
            {
                entity.ToTable("Director");

                entity.HasIndex(e => e.DirectorName, "uk_directorName")
                    .IsUnique();

                entity.Property(e => e.DirectorId)
                    .HasColumnName("directorId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.DirectorName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("directorName");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genre");

                entity.HasIndex(e => e.GenreName, "uk_genreName")
                    .IsUnique();

                entity.Property(e => e.GenreId)
                    .HasColumnName("genreId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.GenreName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("genreName");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Language");

                entity.HasIndex(e => e.LanguageName, "uk_languageName")
                    .IsUnique();

                entity.Property(e => e.LanguageId)
                    .HasColumnName("languageId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LanguageName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("languageName");
            });

            modelBuilder.Entity<Movie>(entity =>
            {
                entity.HasKey(e => e.ImdbId)
                    .HasName("pk_imdbId");

                entity.ToTable("Movie");

                entity.HasIndex(e => e.ImdbId, "uk_imdbId")
                    .IsUnique();

                entity.HasIndex(e => e.Title, "uk_movieTitle")
                    .IsUnique();

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.IsReleased).HasColumnName("isReleased");

                entity.Property(e => e.Plot)
                    .HasColumnType("text")
                    .HasColumnName("plot");

                entity.Property(e => e.RatingId).HasColumnName("ratingId");

                entity.Property(e => e.ReleaseCountry)
                    .HasMaxLength(255)
                    .HasColumnName("releaseCountry");

                entity.Property(e => e.ReleaseDate)
                    .HasColumnType("date")
                    .HasColumnName("releaseDate");

                entity.Property(e => e.RuntimeMinutes).HasColumnName("runtimeMinutes");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.HasOne(d => d.Rating)
                    .WithMany(p => p.Movies)
                    .HasForeignKey(d => d.RatingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_movieRating");
            });

            modelBuilder.Entity<MovieActor>(entity =>
            {
                entity.HasKey(e => new { e.ImdbId, e.ActorId })
                    .HasName("pk_imdbIdAndActorId");

                entity.ToTable("Movie_Actor");

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.ActorId).HasColumnName("actorId");
            });

            modelBuilder.Entity<MovieDirector>(entity =>
            {
                entity.HasKey(e => new { e.ImdbId, e.DirectorId })
                    .HasName("pk_imdbIdDirectorId");

                entity.ToTable("Movie_Director");

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.DirectorId).HasColumnName("directorId");
            });

            modelBuilder.Entity<MovieGenre>(entity =>
            {
                entity.HasKey(e => new { e.ImdbId, e.GenreId })
                    .HasName("pk_movieGenreId");

                entity.ToTable("Movie_Genre");

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.GenreId).HasColumnName("genreId");
            });

            modelBuilder.Entity<MovieLanguage>(entity =>
            {
                entity.HasKey(e => new { e.ImdbId, e.LanguageId })
                    .HasName("pk_movieLanguageId");

                entity.ToTable("Movie_Language");

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.LanguageId).HasColumnName("languageId");
            });

            modelBuilder.Entity<Rating>(entity =>
            {
                entity.ToTable("Rating");

                entity.HasIndex(e => e.RatingName, "uk_ratingName")
                    .IsUnique();

                entity.Property(e => e.RatingId).HasColumnName("ratingId");

                entity.Property(e => e.RatingName)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("ratingName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
