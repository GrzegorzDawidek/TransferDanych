using System.ComponentModel.DataAnnotations.Schema;

namespace TransferDanych.Modele
{
    public class Osoba
    {
        public int Id { get; set; }
        public float Waga { get; set; }
        public DateTime DataUrodzenia { get; set; }
        public string ImieINazwisko { get; set; }
        public string Miasto { get; set; }
        [NotMapped]
        public bool Status { get; set; }
    }
}
