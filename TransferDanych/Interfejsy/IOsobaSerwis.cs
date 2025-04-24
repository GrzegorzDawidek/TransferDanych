using TransferDanych.Modele;

namespace TransferDanych.Interfejsy
{
    internal interface IOsobaSerwis
    {
        Task<List<Osoba>> PobierzOsobyAsync(string ciagPol, string baza);
    }
}
