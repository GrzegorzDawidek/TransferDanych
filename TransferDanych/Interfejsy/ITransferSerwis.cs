using TransferDanych.Modele;

namespace TransferDanych.Interfejsy
{
    internal interface ITransferSerwis
    {
        Task<bool> TransferOsob(List<Osoba> osoby, string ciagBazaDocelowa);
    }
}
