using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OsEngine.ViewModels;

namespace OsEngine.Views
{
    /// <summary>
    /// Логика взаимодействия для ChangeEmitentWindow.xaml
    /// </summary>
    public partial class ChangeEmitentWindow : Window
    {
        public ChangeEmitentWindow()
        {
            InitializeComponent();

            DataContext = new ChangeEmitentVM();
        }
    }
}
