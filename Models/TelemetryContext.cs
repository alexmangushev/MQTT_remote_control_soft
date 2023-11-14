using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace mqtt_remote_server.Models;

public partial class TelemetryContext : DbContext
{
    public TelemetryContext()
    {
    }

    public TelemetryContext(DbContextOptions<TelemetryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Channel> Channels { get; set; }

    public virtual DbSet<Datum> Data { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<DeviceToUser> DeviceToUsers { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    static private StreamReader strReader = new StreamReader("ConnectionDataBase.txt"); // get Reader
    static string conn = strReader.ReadLine();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(conn);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(e => e.ChannelId).HasName("channel_pkey");

            entity.ToTable("channel");

            entity.Property(e => e.ChannelId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, 0L, null, null, null)
                .HasColumnName("channel_id");
            entity.Property(e => e.Operator)
                .HasColumnType("character varying")
                .HasColumnName("operator");
            entity.Property(e => e.Phone)
                .HasColumnType("character varying")
                .HasColumnName("phone");
            entity.Property(e => e.Tariff)
                .HasColumnType("character varying")
                .HasColumnName("tariff");
        });

        modelBuilder.Entity<Datum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("data_pkey");

            entity.ToTable("data");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(2L, null, null, null, null, null)
                .HasColumnName("id");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.GetTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("get_time");
            entity.Property(e => e.Humidity).HasColumnName("humidity");
            entity.Property(e => e.People).HasColumnName("people");
            entity.Property(e => e.Power).HasColumnName("power");
            entity.Property(e => e.Smoke).HasColumnName("smoke");
            entity.Property(e => e.Temp).HasColumnName("temp");

            /*entity.HasOne(d => d.Device).WithMany(p => p.Data)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("device_id_pkey");*/
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("device_pkey");

            entity.ToTable("device");

            entity.Property(e => e.DeviceId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, 0L, null, null, null)
                .HasColumnName("device_id");
            entity.Property(e => e.ChannelId).HasColumnName("channel_id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");

            entity.HasOne(d => d.Channel).WithMany(p => p.Devices)
                .HasForeignKey(d => d.ChannelId)
                .HasConstraintName("channel_in_fkey");
        });

        modelBuilder.Entity<DeviceToUser>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.DeviceId }).HasName("pk");

            entity.ToTable("device_to_user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.DeviceId).HasColumnName("device_id");
            entity.Property(e => e.Info)
                .HasColumnType("character varying")
                .HasColumnName("info");

            entity.HasOne(d => d.Device).WithMany(p => p.DeviceToUsers)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("device_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.DeviceToUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id_fkey");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("transactions_pkey");

            entity.ToTable("transactions");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Amount)
                .HasPrecision(10, 2)
                .HasColumnName("amount");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.TransactionDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("transaction_date");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.ToTable("user");

            entity.HasIndex(e => e.Login, "constraintLogin").IsUnique();

            entity.Property(e => e.UserId)
                .UseIdentityAlwaysColumn()
                .HasIdentityOptions(null, null, 0L, null, null, null)
                .HasColumnName("user_id");
            entity.Property(e => e.IsAdmin).HasColumnName("is_admin");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
