using Microsoft.EntityFrameworkCore;
using System.Windows;
using TransferDanych.Dane;
using TransferDanych.Interfejsy;
using TransferDanych.Modele;

namespace TransferDanych.Serwisy
{
    internal class TransferSerwis : ITransferSerwis
    {
        public async Task<bool> TransferOsob(List<Osoba> osoby, string ciagPol)
        {
            using var bazaDanych = new BazaDanych(ciagPol);
            bazaDanych.Database.OpenConnection();
            using var tranzakcja = bazaDanych.Database.BeginTransaction();

            try
            {
                await bazaDanych.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Osoba ON");
                bazaDanych.Osoba.AddRange(osoby);
                await bazaDanych.SaveChangesAsync();
                await bazaDanych.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Osoba OFF");
                tranzakcja.Commit();
                return true;
            }
            catch
            {
                tranzakcja.Rollback();
                MessageBox.Show("Wystąpił błąd poczas przesyłania danych.\n Upewnnij się, że połączenie z serwerem nie zostało przerwane.", $"Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
