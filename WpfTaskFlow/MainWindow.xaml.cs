using System.Windows;
using WpfTaskFlow.ViewModels;

namespace WpfTaskFlow
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}