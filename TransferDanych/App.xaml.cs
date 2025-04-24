using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TransferDanych.Interfejsy;
using TransferDanych.Komunikaty;
using TransferDanych.Modele;
using TransferDanych.Serwisy;
using TransferDanych.ViewModels;

namespace TransferDanych
{
    public partial class App : Application
    {
        private IServiceProvider _dostawcaUslug;
        private IConfiguration _konfiguracja;

        public App()
        {
            var konfiguracja = new ConfigurationBuilder()
                 .SetBasePath(AppContext.BaseDirectory)
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _konfiguracja = konfiguracja.Build();

            var kolekcjaUslug = new ServiceCollection();
            SkonfigurujUslugi(kolekcjaUslug);
            _dostawcaUslug = kolekcjaUslug.BuildServiceProvider();
        }
        private void SkonfigurujUslugi(IServiceCollection uslugi)
        {
            SkonfigurujUstawienia(uslugi);
            SkonfigurujSerwisy(uslugi);
            SkonfigurujModeleWidoku(uslugi);
            SkonfigurujWidoki(uslugi);
        }
        private void SkonfigurujUstawienia(IServiceCollection uslugi)
        {
            uslugi.AddSingleton<IConfiguration>(_konfiguracja);
            uslugi.Configure<DaneStartoweConfig>(_konfiguracja.GetSection("DaneStartowe"));
        }
        private void SkonfigurujWidoki(IServiceCollection uslugi)
        {
            uslugi.AddTransient<MainWindow>();
        }
        private void SkonfigurujModeleWidoku(IServiceCollection uslugi)
        {
            uslugi.AddSingleton<IMainViewModel, MainViewModel>();
        }
        private void SkonfigurujSerwisy(IServiceCollection uslugi)
        {
            uslugi.AddSingleton<IMessageDialogService, ObslugaKomunikatow>();
            uslugi.AddSingleton<IBazaDanychSerwis, BazyDanychSerwis>();
            uslugi.AddScoped<IOsobaSerwis, OsobaSerwis>();
            uslugi.AddTransient<ITransferSerwis, TransferSerwis>();
        }
        private void UruchomienieAplikacji(object sender, StartupEventArgs e)
        {
            var oknoGlowne = _dostawcaUslug.GetService<MainWindow>();
            oknoGlowne.DataContext = _dostawcaUslug.GetService<IMainViewModel>();
            oknoGlowne.Show();
        }
    }

}
