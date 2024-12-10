using Microsoft.EntityFrameworkCore;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.DataAccess;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<DownloadDto>().Ignore(x => x.Version);
        builder.Entity<DownloadDto>().ToTable("downloads");

        builder.Entity<DownloadAuthorDto>().HasKey(e => new { e.UserId, e.DownloadId });
        builder.Entity<DownloadAuthorDto>()
            .HasOne(e => e.Download)
            .WithMany(e => e.Authors)
            .HasForeignKey(e => e.DownloadId)
            .HasPrincipalKey(e => e.Id);
        builder.Entity<DownloadAuthorDto>().ToTable("authors");

        builder.Entity<DownloadFileDto>().HasIndex(x => x.Filename);
        builder.Entity<DownloadFileDto>().ToTable("files");

        builder.Entity<FileDownloadDto>().ToTable("file_downloads");

        builder.Entity<CommentDto>().HasIndex(x => x.UserId);
        builder.Entity<CommentDto>().ToTable("comments");

        builder.Entity<ReplyDto>().HasKey(e => new { e.ParentId, e.ReplyId });
        
        builder.Entity<ReplyDto>()
            .HasOne(e => e.Parent)
            .WithMany(e => e.Replies);

        builder.Entity<ReplyDto>()
            .ToTable("replies");

        builder.Entity<DownloadCommentDto>().HasKey(e => new { e.DownloadId, e.CommentId });
        
        builder.Entity<DownloadCommentDto>()
            .HasOne(e => e.Comment)
            .WithOne(e => e.Download);
        
        builder.Entity<DownloadCommentDto>().ToTable("download_comments");
    }
}