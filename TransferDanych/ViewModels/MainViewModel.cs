using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.ObjectModel;
using TransferDanych.Dane;
using TransferDanych.Interfejsy;
using TransferDanych.Komunikaty;
using TransferDanych.Modele;

namespace TransferDanych.ViewModels
{
    internal partial class MainViewModel : ObservableObject, IMainViewModel
    {
        #region Pola prywatne
        private readonly IMessageDialogService _komunikaty;
        private readonly ITransferSerwis _transferSerwis;
        private readonly IOsobaSerwis _osobaSerwis;
        private readonly IBazaDanychSerwis _dbSerwis;
        private string _ciagPolaczeniaZrodlowego, _ciagPolaczeniaDocelowego;
        private int _aktualnaStrona = 1, _rozmiarStrony = 10, _liczbaRekordow;
        #endregion
        #region Właściwości
        public string WybranaBazaD { get; set; }
        public string WybranaBazaZ { get; set; }
        public string NazwaSerwera { get; set; }
        public string Login { get; set; }
        private string _haslo;
        public string Haslo
        {
            get => _haslo;
            set
            {
                _haslo = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> TypyUwierzytelniania { get; set; }
        private ObservableCollection<Osoba> _listaOsobPodglad;
        public ObservableCollection<Osoba> ListaOsobPodglad
        {
            get => _listaOsobPodglad;
            set => SetProperty(ref _listaOsobPodglad, value);
        }
        private ObservableCollection<Osoba> _listaOsobPrzesylka;
        public ObservableCollection<Osoba> ListaOsobPrzesylka
        {
            get => _listaOsobPrzesylka;
            set => SetProperty(ref _listaOsobPrzesylka, value);
        }
        private bool _czyWidocznaKonfiguracjaPolaczenia = true;
        public bool CzyWidocznaKonfiguracjaPolaczenia
        {
            get => _czyWidocznaKonfiguracjaPolaczenia;
            set => SetProperty(ref _czyWidocznaKonfiguracjaPolaczenia, value);
        }
        private bool _czyMoznaPrzeslac;
        public bool CzyMoznaPrzeslac
        {
            get => _czyMoznaPrzeslac;
            set => SetProperty(ref _czyMoznaPrzeslac, value);
        }
        private bool _czyMoznaPobrac = true;
        public bool CzyMoznaPobrac
        {
            get => _czyMoznaPobrac;
            set => SetProperty(ref _czyMoznaPobrac, value);
        }
        private bool _czyPodgladAktywny = false;
        public bool CzyPodgladAktywny
        {
            get => _czyPodgladAktywny;
            set => SetProperty(ref _czyPodgladAktywny, value);
        }
        private bool _czyPrzesylanieAktywne = false;
        public bool CzyPrzesylanieAktywne
        {
            get => _czyPrzesylanieAktywne;
            set => SetProperty(ref _czyPrzesylanieAktywne, value);
        }
        public int AktualnaStrona
        {
            get => _aktualnaStrona;
            set => SetProperty(ref _aktualnaStrona, value);
        }
        private int _liczbaStron = 1;
        public int LiczbaStron
        {
            get => _liczbaStron;
            set => SetProperty(ref _liczbaStron, value);
        }
        private bool _czyMoznaWstecz;
        public bool CzyMoznaWstecz
        {
            get => _czyMoznaWstecz;
            set => SetProperty(ref _czyMoznaWstecz, value);
        }
        private bool _czyMoznaDalej;
        public bool CzyMoznaDalej
        {
            get => _czyMoznaDalej;
            set => SetProperty(ref _czyMoznaDalej, value);
        }
        private string _infoOStronie;
        public string InfoOStronie
        {
            get => _infoOStronie;
            set => SetProperty(ref _infoOStronie, value);
        }
        private bool _czyStronaZaladowana;
        public bool CzyStronaZaladowana
        {
            get => _czyStronaZaladowana;
            set => SetProperty(ref _czyStronaZaladowana, value);
        }
        private string _typUwierzytelniania = "Windows Authentication";
        public string TypUwierzytelniania
        {
            get => _typUwierzytelniania;
            set
            {
                _typUwierzytelniania = value;
                OnPropertyChanged();
                CzyPolaLogowaniaAktywne = value == "SQL Server Authentication";
            }
        }
        private bool _czyPolaLogowaniaAktywne;
        public bool CzyPolaLogowaniaAktywne
        {
            get => _czyPolaLogowaniaAktywne;
            set => SetProperty(ref _czyPolaLogowaniaAktywne, value);
        }
        #endregion
        public MainViewModel(IOptions<DaneStartoweConfig> config, IMessageDialogService komunikaty, IBazaDanychSerwis dbSerwis,
            IOsobaSerwis osobaSerwis, ITransferSerwis transferSerwis)
        {
            _komunikaty = komunikaty;
            _dbSerwis = dbSerwis;
            _osobaSerwis = osobaSerwis;
            _transferSerwis = transferSerwis;
            WybranaBazaZ = config.Value.BazaZ;
            WybranaBazaD = config.Value.BazaD;
            NazwaSerwera = config.Value.NazwaSerwera;
            TypyUwierzytelniania = new ObservableCollection<string>(config.Value.TypyUwierzytelniania);
        }
        [RelayCommand]
        private async Task UtworzPolaczeniaITabele()
        {
            bool czyPolaczono = false;
            if (string.IsNullOrEmpty(WybranaBazaD))
            {
                _komunikaty.WyswietlKomunikat("Nie wybrano bazy docelowej.", "error");
                return;
            }
            if (string.IsNullOrEmpty(WybranaBazaZ))
            {
                _komunikaty.WyswietlKomunikat("Nie wybrano bazy źródłowej.", "error");
                return;
            }
            if (string.IsNullOrEmpty(NazwaSerwera))
            {
                _komunikaty.WyswietlKomunikat("Nie zdefiniowano serwera", "error");
                return;
            }
            if (WybranaBazaD.Equals(WybranaBazaZ))
            {
                _komunikaty.WyswietlKomunikat("Wybrano dwie takie same bazy. Operacja zabroniona.", "error");
                return;
            }
            UstawStanKontrolek(new Dictionary<string, bool> { { nameof(CzyWidocznaKonfiguracjaPolaczenia), false } ,{nameof(CzyPolaLogowaniaAktywne), false }});

            _ciagPolaczeniaZrodlowego = StworzCiagPolaczenia(WybranaBazaZ, Login, Haslo, TypUwierzytelniania);
            _ciagPolaczeniaDocelowego = StworzCiagPolaczenia(WybranaBazaD, Login, Haslo, TypUwierzytelniania);

            try
            {
                if (await _dbSerwis.TestPolaczenia(_ciagPolaczeniaZrodlowego) && await _dbSerwis.TestPolaczenia(_ciagPolaczeniaDocelowego))
                {
                    await _dbSerwis.TworzenieBaz(_ciagPolaczeniaZrodlowego, _ciagPolaczeniaDocelowego);
                    czyPolaczono = true;
                    UstawStanKontrolek(new Dictionary<string, bool>{{ nameof(CzyPodgladAktywny), true },
                        { nameof(CzyPrzesylanieAktywne), true }});

                    _komunikaty.WyswietlKomunikat("Połączenia zostały nawiązane pomyślnie.", "info");
                }
                else
                {
                    _komunikaty.WyswietlKomunikat("Nie udało się nawiązać połączenia z jedną z baz danych.", "error");
                }
            }
            catch (SqlException)
            {
                _komunikaty.WyswietlKomunikat("Wystąpił problem z bazą danych.\nSprawdź serwer lub pliki baz danych.", "error");
            }
            catch
            {
                _komunikaty.WyswietlKomunikat("Wystąpił nieoczekiwany błąd podczas próby połączenia.", "error");
            }
            if (!czyPolaczono)
                UstawStanKontrolek(new Dictionary<string, bool> { { nameof(CzyWidocznaKonfiguracjaPolaczenia), true }
                    ,{nameof(CzyPolaLogowaniaAktywne), TypUwierzytelniania == "SQL Server Authentication" ? true : false } });
        }
        private string StworzCiagPolaczenia(string baza, string login, string haslo, string typUwierzytelniania)
        {
            if (typUwierzytelniania == "Windows Authentication")
            {
                return $"Data Source={NazwaSerwera};Initial catalog={baza};Integrated Security=True;";
            }
            else
            {
                return $"Data Source={NazwaSerwera};Initial catalog={baza};User ID = {login}; Password = {haslo}";
            }
        }
        private void UstawStanKontrolek(Dictionary<string, bool> stanyKontrolek)
        {
            foreach (var stanKontrolki in stanyKontrolek)
            {
                GetType().GetProperty(stanKontrolki.Key)!.SetValue(this, stanKontrolki.Value);
            }
        }
        [RelayCommand]
        private void PobierzPierwszaStrone() { ZaladujStrone(1); CzyStronaZaladowana = true; }
        [RelayCommand]
        private void PobierzPoprzedniaStrone() { if (AktualnaStrona > 1) { ZaladujStrone(AktualnaStrona - 1); } }
        [RelayCommand]
        private void PobierzNastepnaStrone() { if (_aktualnaStrona < _liczbaStron) ZaladujStrone(_aktualnaStrona + 1); }
        [RelayCommand]
        private void PobierzOstatniaStrone() { ZaladujStrone(_liczbaStron); }
        [RelayCommand]
        public async Task PobierzDane() => await PobierzDaneZrodlowe();
        [RelayCommand]
        public async Task PrzeslijZaznaczone(IList zaznaczoneElementy)
            => await PrzeslijDane(zaznaczoneElementy.Cast<Osoba>().Where(o => !Convert.ToBoolean(o.Status)).ToList());
        [RelayCommand]
        public async Task PrzeslijWszystkie() => await PrzeslijDane(ListaOsobPrzesylka.Where(o => !Convert.ToBoolean(o.Status)).ToList());
        private async Task PrzeslijDane(IList<Osoba> wybraneOsoby)
        {
            try
            {
                UstawStanKontrolek(new Dictionary<string, bool> { { nameof(CzyMoznaPobrac), false }, { nameof(CzyMoznaPrzeslac), false } });

                if (wybraneOsoby.Count == 0)
                {
                    _komunikaty.WyswietlKomunikat("Nie ma nowych osób do przesłania.", "info");
                    return;
                }
                var czySiePowiodlo = await _transferSerwis.TransferOsob(wybraneOsoby.ToList(), _ciagPolaczeniaDocelowego);

                if (!czySiePowiodlo)
                {
                    return;
                }
                await PobierzDaneZrodlowe();
                _komunikaty.WyswietlKomunikat("Wszystkie nowe osoby zostały poprawnie przesłane.", "info");
            }
            catch
            {
                _komunikaty.WyswietlKomunikat("Wystąpił problem podczas przesyłania danych.\nUpewnij się, że baza docelowa jest dostępna.", "error");
            }
            finally
            {
                UstawStanKontrolek(new Dictionary<string, bool> { { nameof(CzyMoznaPobrac), true }, { nameof(CzyMoznaPrzeslac), true } });
            }
        }
        private void ZaladujStrone(int numerStrony)
        {
            try
            {
                using var context = new BazaDanych(_ciagPolaczeniaDocelowego);
                _liczbaRekordow = context.Osoba.Count();
                _liczbaStron = Math.Max(1, (_liczbaRekordow + _rozmiarStrony - 1) / _rozmiarStrony);

                var osoby = context.Osoba.OrderBy(o => o.Id)
                    .Skip((numerStrony - 1) * _rozmiarStrony)
                    .Take(_rozmiarStrony)
                    .ToList();

                ListaOsobPodglad = new ObservableCollection<Osoba>(osoby);
                AktualnaStrona = numerStrony;

                CzyMoznaWstecz = numerStrony > 1;
                CzyMoznaDalej = numerStrony < _liczbaStron;
                InfoOStronie = $"Strona {numerStrony} z {_liczbaStron}";
            }
            catch
            {
                _komunikaty.WyswietlKomunikat("Nie udało się załadować danych.\nUpewnij się, że połączenie z bazą danych działa poprawnie.", "error");
            }
        }
        private async Task PobierzDaneZrodlowe()
        {
            try
            {
                ListaOsobPrzesylka = new ObservableCollection<Osoba>(await _osobaSerwis.PobierzOsobyAsync(_ciagPolaczeniaZrodlowego, WybranaBazaD));
                CzyMoznaPrzeslac = true;
            }
            catch
            {
                _komunikaty.WyswietlKomunikat("Nie udało się pobrać danych z bazy źródłowej.\n Sprawdź połączenie z serwerem.", "error");
            }
        }
    }
}
