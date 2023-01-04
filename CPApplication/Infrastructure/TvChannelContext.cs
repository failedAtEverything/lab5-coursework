using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CPApplication.Core.Models;

namespace CPApplication.Infrastructure;

public partial class TvChannelContext : DbContext
{
    public TvChannelContext()
    {
    }

    public TvChannelContext(DbContextOptions<TvChannelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appeal> Appeals { get; set; }

    public virtual DbSet<Broadcast> Broadcasts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<TvProgram> TvPrograms { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=SUSSY-BAKA\\SQLEXPRESS;Initial Catalog=TV_Channel;Integrated Security=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appeal>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AppealPurpose)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("appealPurpose");
            entity.Property(e => e.BroadcastId).HasColumnName("broadcastId");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Organization)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("organization");

            entity.HasOne(d => d.Broadcast).WithMany(p => p.Appeals)
                .HasForeignKey(d => d.BroadcastId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appeals_Appeals");
        });

        modelBuilder.Entity<Broadcast>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_WeeklyProgramList");

            entity.HasIndex(e => new { e.WeekNumber, e.WeekMonth, e.WeekYear }, "IX_WeeklyProgramList");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EmployeesId).HasColumnName("employeesId");
            entity.Property(e => e.EndTime).HasColumnName("endTime");
            entity.Property(e => e.Guests)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("guests");
            entity.Property(e => e.ProgramId).HasColumnName("programId");
            entity.Property(e => e.StartTime).HasColumnName("startTime");
            entity.Property(e => e.WeekMonth).HasColumnName("weekMonth");
            entity.Property(e => e.WeekNumber).HasColumnName("weekNumber");
            entity.Property(e => e.WeekYear).HasColumnName("weekYear");

            entity.HasOne(d => d.Employees).WithMany(p => p.Broadcasts)
                .HasForeignKey(d => d.EmployeesId)
                .HasConstraintName("FK_Broadcasts_Employees");

            entity.HasOne(d => d.Program).WithMany(p => p.Broadcasts)
                .HasForeignKey(d => d.ProgramId)
                .HasConstraintName("FK_Broadcasts_Programs");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Position)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("position");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TvProgram>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Programs");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.GenreId).HasColumnName("genreId");
            entity.Property(e => e.Length)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("length");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Rating).HasColumnName("rating");

            entity.HasOne(d => d.Genre).WithMany(p => p.TvPrograms)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK_Programs_Genres");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
