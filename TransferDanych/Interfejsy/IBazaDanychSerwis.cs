namespace TransferDanych.Interfejsy
{
    internal interface IBazaDanychSerwis
    {
        Task TworzenieBaz(string zrodlowyCiagPol, string docelowyCiagpol);
        Task<bool> TestPolaczenia(string ciagPol);
    }
}
