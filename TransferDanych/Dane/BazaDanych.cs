using Microsoft.EntityFrameworkCore;
using TransferDanych.Modele;

namespace TransferDanych.Dane
{
    public class BazaDanych : DbContext
    {
        private readonly string _ciagPolaczenia;
        public BazaDanych(string ciagPolaczenia)
        {
            _ciagPolaczenia = ciagPolaczenia;
        }
        public DbSet<Osoba> Osoba { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_ciagPolaczenia);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Osoba>()
                .Property(o => o.DataUrodzenia)
                .HasColumnType("datetime2");
            modelBuilder.Entity<Osoba>()
                .Property(o => o.Waga)
                .HasColumnType("float");
            modelBuilder.Entity<Osoba>()
                .Property(o => o.ImieINazwisko)
                .HasColumnType("VARCHAR(100)");
            modelBuilder.Entity<Osoba>()
                .Property(o => o.Miasto)
                .HasColumnType("NVARCHAR(100)");
        }
    }
}
