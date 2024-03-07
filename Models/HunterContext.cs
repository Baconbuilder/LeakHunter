using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace LeakHunter.Models
{
    public partial class HunterContext : DbContext
    {
        public HunterContext()
        {
        }

        public HunterContext(DbContextOptions<HunterContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Analysis> Analyses { get; set; } = null!;
        public virtual DbSet<Announment> Announments { get; set; } = null!;
        public virtual DbSet<Hunter> Hunters { get; set; } = null!;
        public virtual DbSet<Interval> Intervals { get; set; } = null!;
        public virtual DbSet<Manager> Managers { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Type> Types { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=127.0.0.1;Database=Hunter;Trusted_Connection=True;User ID=sa;Password=1234");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Analysis>(entity =>
            {
                entity.HasKey(e => e.AId);

                entity.ToTable("Analysis");

                entity.Property(e => e.AId).HasColumnName("A_ID");

                entity.Property(e => e.CId).HasColumnName("C_ID");

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.TId).HasColumnName("T_ID");
            });

            modelBuilder.Entity<Announment>(entity =>
            {
                entity.HasKey(e => e.AnnoId);

                entity.ToTable("Announment");

                entity.Property(e => e.AnnoId).HasColumnName("Anno_ID");

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<Hunter>(entity =>
            {
                entity.HasKey(e => e.CId);

                entity.ToTable("Hunter");

                entity.Property(e => e.CId).HasColumnName("C_ID");

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.Property(e => e.UId).HasColumnName("U_ID");

                entity.Property(e => e.Url)
                    .IsUnicode(false)
                    .HasColumnName("URL");

                entity.Property(e => e.WebName)
                    .HasMaxLength(50)
                    .HasColumnName("Web_name");
            });

            modelBuilder.Entity<Interval>(entity =>
            {
                entity.HasKey(e => e.IId);

                entity.ToTable("Interval");

                entity.Property(e => e.IId).HasColumnName("I_ID");

                entity.Property(e => e.Next).HasColumnType("datetime");

                entity.Property(e => e.UId).HasColumnName("U_ID");

                entity.Property(e => e.Url)
                    .HasColumnType("text")
                    .HasColumnName("URL");

                entity.Property(e => e.WebName).HasMaxLength(50);
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasKey(e => e.MId);

                entity.ToTable("Manager");

                entity.Property(e => e.MId).HasColumnName("M_ID");

                entity.Property(e => e.Account).HasMaxLength(50);

                entity.Property(e => e.MName)
                    .HasMaxLength(50)
                    .HasColumnName("M_Name");

                entity.Property(e => e.MPassword)
                    .HasColumnType("text")
                    .HasColumnName("M_Password");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.MesId);

                entity.ToTable("Message");

                entity.Property(e => e.MesId).HasColumnName("Mes_ID");

                entity.Property(e => e.Content).HasColumnType("text");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Title).HasMaxLength(50);

                entity.Property(e => e.UId).HasColumnName("U_ID");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.HasKey(e => e.TId);

                entity.ToTable("Type");

                entity.Property(e => e.TId)
                    .ValueGeneratedNever()
                    .HasColumnName("T_ID");

                entity.Property(e => e.Datatype).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UId);

                entity.ToTable("User");

                entity.Property(e => e.UId).HasColumnName("U_ID");

                entity.Property(e => e.Permission)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Phone_Number");

                entity.Property(e => e.UEmail)
                    .HasMaxLength(50)
                    .HasColumnName("U_Email");

                entity.Property(e => e.UName)
                    .HasMaxLength(50)
                    .HasColumnName("U_Name");

                entity.Property(e => e.UPassword)
                    .HasMaxLength(50)
                    .HasColumnName("U_Password");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
