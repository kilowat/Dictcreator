﻿using Dictcreator.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            var result = Parser.RunAsync();
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
                AppSettings.Instance.FileXlsPath = _openFielDialog.FileName;
            }
        }

        private void InitApp()
        {
            _openFielDialog = new OpenFileDialog();
            _openFielDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            AppSettings.Instance.ColNumberIndex = colNumberIndex.Text;
            AppSettings.Instance.ColEnWord = colEnWord.Text;
            AppSettings.Instance.ColTranscript = colTranscript.Text;
            AppSettings.Instance.ColTranslation = colTranslation.Text;
            AppSettings.Instance.ColExamples = colExamples.Text;
            AppSettings.Instance.MaxExample = Int32.Parse(maxExample.Text);
            AppSettings.Instance.ColAudio = colAudio.Text;
            AppSettings.Instance.ColYouglish = colYouglish.Text;
            AppSettings.Instance.ColContextReverso = colContextReverso.Text;
            AppSettings.Instance.ColWoordHunt = colWoordHunt.Text;
            AppSettings.Instance.ColMerWebster = colMerWebster.Text;
            AppSettings.Instance.StartNumberIndex = Int32.Parse(startNumberIndex.Text);
            AppSettings.Instance.EndNumberIndex = Int32.Parse(endNumberIndex.Text);
        }

        private void ColumnSettings_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            var match = Regex.Match(textBox.Text, "[A-Z]");

            if (textBox.Text.Length > 0)
            {
                if (!match.Success || textBox.Text.Length > 1)
                {
                    textBox.Text = "A";
                    MessageBox.Show("Значение должно быть в диапозоне A-Z", "Ошибка ввода");
                }
            }
            else if(textBox.Name == "colEnWord")
            {
                textBox.Text = "B";
                MessageBox.Show("Значение колонка слово должно быть указано", "Ошибка ввода");
            }
        }
    }
}
