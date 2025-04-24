using Dapper;
using Microsoft.Data.SqlClient;
using TransferDanych.Interfejsy;
using TransferDanych.Modele;

namespace TransferDanych.Serwisy
{
    internal class OsobaSerwis : IOsobaSerwis
    {
        public async Task<List<Osoba>> PobierzOsobyAsync(string ciagPol, string baza)
        {
            var sql = $@"
                    SELECT  o.*,
                           IIF(d.Id IS NOT NULL, 1, 0) AS Status
                    FROM dbo.Osoba (nolock) o
                    LEFT JOIN {baza}.dbo.Osoba (nolock) d ON o.Id = d.Id";
            await using var conn = new SqlConnection(ciagPol);
            return (await conn.QueryAsync<Osoba>(sql)).ToList();
        }
    }
}
