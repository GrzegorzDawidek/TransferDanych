using System.Windows;
using TransferDanych.ViewModels;

namespace TransferDanych
{
    public partial class MainWindow : Window
    {
        public MainWindow(IMainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = mainViewModel;
        }
    }
}