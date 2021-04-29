using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Repository.Models
{
    /// <summary>
    /// Defines the context for the Movie database.
    /// </summary>
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
        public virtual DbSet<FollowingMovie> FollowingMovies { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<MovieActor> MovieActors { get; set; }
        public virtual DbSet<MovieDirector> MovieDirectors { get; set; }
        public virtual DbSet<MovieGenre> MovieGenres { get; set; }
        public virtual DbSet<MovieLanguage> MovieLanguages { get; set; }
        public virtual DbSet<MovieTag> MovieTags { get; set; }
        public virtual DbSet<MovieTagUser> MovieTagUsers { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { }

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

            modelBuilder.Entity<FollowingMovie>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.ImdbId })
                    .HasName("user_following_movie_pk");

                entity.ToTable("Following_Movies");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("userID");

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbID");

                entity.HasOne(d => d.Imdb)
                    .WithMany(p => p.FollowingMovies)
                    .HasForeignKey(d => d.ImdbId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Following__imdbI__40058253");
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

                entity.HasIndex(e => e.Title, "uk_movieTitle")
                    .IsUnique();

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.IsReleased).HasColumnName("isReleased");

                entity.Property(e => e.Plot).HasColumnName("plot");

                entity.Property(e => e.PosterUrl)
                    .HasMaxLength(2048)
                    .HasColumnName("posterURL");

                entity.Property(e => e.RatingId).HasColumnName("ratingId");

                entity.Property(e => e.ReleaseCountry)
                    .HasMaxLength(255)
                    .HasColumnName("releaseCountry");

                entity.Property(e => e.ReleaseDate)
                    .HasColumnType("date")
                    .HasColumnName("releaseDate");

                entity.Property(e => e.RuntimeMinutes).HasColumnName("runtimeMinutes");

                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .HasColumnName("title");

                entity.HasOne(d => d.Rating)
                    .WithMany(p => p.Movies)
                    .HasForeignKey(d => d.RatingId)
                    .HasConstraintName("fk_movierating");
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

                entity.HasOne(d => d.Actor)
                    .WithMany(p => p.MovieActors)
                    .HasForeignKey(d => d.ActorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_actorId");

                entity.HasOne(d => d.Imdb)
                    .WithMany(p => p.MovieActors)
                    .HasForeignKey(d => d.ImdbId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_actor_imdbId");
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

                entity.HasOne(d => d.Director)
                    .WithMany(p => p.MovieDirectors)
                    .HasForeignKey(d => d.DirectorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_directorId");

                entity.HasOne(d => d.Imdb)
                    .WithMany(p => p.MovieDirectors)
                    .HasForeignKey(d => d.ImdbId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_director_imdbId");
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

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.MovieGenres)
                    .HasForeignKey(d => d.GenreId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_genreId");

                entity.HasOne(d => d.Imdb)
                    .WithMany(p => p.MovieGenres)
                    .HasForeignKey(d => d.ImdbId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_genre_imdbId");
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

                entity.HasOne(d => d.Imdb)
                    .WithMany(p => p.MovieLanguages)
                    .HasForeignKey(d => d.ImdbId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_language_imdbId");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.MovieLanguages)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_languageId");
            });

            modelBuilder.Entity<MovieTag>(entity =>
            {
                entity.HasKey(e => new { e.ImdbId, e.TagName })
                    .HasName("pk_imdbId_tagName");

                entity.ToTable("Movie_Tag");

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.TagName)
                    .HasMaxLength(50)
                    .HasColumnName("tagName");

                entity.Property(e => e.VoteSum).HasColumnName("voteSum");

                entity.HasOne(d => d.Imdb)
                    .WithMany(p => p.MovieTags)
                    .HasForeignKey(d => d.ImdbId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_movietag_imdbId");

                entity.HasOne(d => d.TagNameNavigation)
                    .WithMany(p => p.MovieTags)
                    .HasForeignKey(d => d.TagName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_movietag_tagName");
            });

            modelBuilder.Entity<MovieTagUser>(entity =>
            {
                entity.HasKey(e => new { e.ImdbId, e.TagName, e.UserId })
                    .HasName("pk_imdbId_tagName_userId");

                entity.ToTable("Movie_Tag_User");

                entity.Property(e => e.ImdbId)
                    .HasMaxLength(255)
                    .HasColumnName("imdbId");

                entity.Property(e => e.TagName)
                    .HasMaxLength(50)
                    .HasColumnName("tagName");

                entity.Property(e => e.UserId)
                    .HasMaxLength(50)
                    .HasColumnName("userId");

                entity.Property(e => e.IsUpvote)
                    .IsRequired()
                    .HasColumnName("is_upvote")
                    .HasDefaultValueSql("((1))");

                entity.HasOne(d => d.Imdb)
                    .WithMany(p => p.MovieTagUsers)
                    .HasForeignKey(d => d.ImdbId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_movietaguser_imdbId");

                entity.HasOne(d => d.TagNameNavigation)
                    .WithMany(p => p.MovieTagUsers)
                    .HasForeignKey(d => d.TagName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_movietaguser_tagName");
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

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.TagName)
                    .HasName("pk_tagName");

                entity.ToTable("Tag");

                entity.Property(e => e.TagName)
                    .HasMaxLength(50)
                    .HasColumnName("tagName");

                entity.Property(e => e.IsBanned).HasColumnName("isBanned");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
