using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Dictcreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenFileDialog _openFielDialog;
        public MainWindow()
        {
            InitializeComponent();
            InitApp();
        }

        private void ClickStartButton(object sender, RoutedEventArgs e)
        {

        }

        private void ClickCancelButton(object sender, RoutedEventArgs e)
        {

        }

        private void ClickSelectTableButton(object sender, RoutedEventArgs e)
        {
            if (_openFielDialog.ShowDialog() == true)
            {
                var arrPath = _openFielDialog.FileName.Split('\\');
                selectFielTextBox.Text = arrPath.Last();
            }
        }

        private void InitApp()
        {
            _openFielDialog = new OpenFileDialog();
            _openFielDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
        }
    }
}
