using Bogus;
using TransferDanych.Modele;

namespace TransferDanych.Inicjalizatory
{
    public static class GeneratorDanych
    {
        public static List<Osoba> GenerujDane(int ilosc)
        {
            var dane = new Faker<Osoba>()
                .RuleFor(o => o.ImieINazwisko, f => f.Name.FullName())
                .RuleFor(o => o.Waga, f => Convert.ToSingle(Math.Round(f.Random.Float(50, 120), 2)))
                .RuleFor(o => o.DataUrodzenia, f => Convert.ToDateTime(f.Date.Past(30).ToString("dd.MM.yyyy HH:mm")))
                .RuleFor(o => o.Miasto, f => f.Address.City());
            var listaOsob = dane.Generate(ilosc);
            return listaOsob;
        }
    }
}
