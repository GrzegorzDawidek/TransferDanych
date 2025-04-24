using System.Windows;

namespace TransferDanych.Komunikaty
{
    internal class ObslugaKomunikatow : IMessageDialogService
    {
        public void WyswietlKomunikat(string wiadomosc, string rodzaj)
        {
            if (rodzaj.Equals("info"))
                MessageBox.Show(wiadomosc, $"Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
            else if (rodzaj.Equals("error"))
                MessageBox.Show(wiadomosc, $"Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
