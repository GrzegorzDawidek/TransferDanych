using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TransferDanych.Dane;
using TransferDanych.Inicjalizatory;
using TransferDanych.Interfejsy;

namespace TransferDanych.Serwisy
{
    internal class BazyDanychSerwis : IBazaDanychSerwis
    {
        public async Task TworzenieBaz(string zrodlowyCiagPol, string docelowyCiagpol)
        {
            using (var db = new BazaDanych(zrodlowyCiagPol))
            {
                if(await db.Database.CanConnectAsync())
                {
                    await db.Database.EnsureCreatedAsync();
                    if (!db.Osoba.Any())
                    {
                        await db.Database.OpenConnectionAsync();
                        db.Osoba.AddRange(GeneratorDanych.GenerujDane(50));
                        await db.SaveChangesAsync();
                    }
                }
            }
            using (var db = new BazaDanych(docelowyCiagpol))
            {
                if(await db.Database.CanConnectAsync())
                    await db.Database.EnsureCreatedAsync();
            }
        }
        public async Task<bool> TestPolaczenia(string ciagPol)
        {
            try
            {
                using var conn = new SqlConnection(ciagPol);
                await conn.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
