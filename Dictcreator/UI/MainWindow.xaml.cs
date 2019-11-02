using Dictcreator.Core;
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
        private Parser _parser;
        public MainWindow()
        {
            InitializeComponent();
            InitApp();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_parser.IsWorked)
            {
                _parser.StopOperation();
            }

            base.OnClosing(e);
        }

        private void ClickStartButton(object sender, RoutedEventArgs e)
        {
            InitSettings();
            _parser.InitParser();
            var result = _parser.RunAsync();
        }

        private void ClickCancelButton(object sender, RoutedEventArgs e)
        {
            _parser.CancelOperation();
        }

        private void ClickSelectTableButton(object sender, RoutedEventArgs e)
        {
            if (_openFielDialog.ShowDialog() == true)
            {
                var arrPath = _openFielDialog.FileName.Split('\\');
                selectFielTextBox.Text = arrPath.Last();
                AppSettings.Instance.FileXlsPath = _openFielDialog.FileName;
                startButton.IsEnabled = true;
                startButton.Foreground = new SolidColorBrush(Colors.White);
            }
        }

        private void InitApp()
        {
            _openFielDialog = new OpenFileDialog();
            _openFielDialog.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";

            _parser = new Parser();
            _parser.OnProcessStared += OnProcessStartedHandler;
            _parser.OnProcessCompleted += OnProcessCompletedHandler;
            _parser.OnProcessIndexStep += OnProcessIndexStepHandler;
            _parser.OnProcessWordStep += OnProcessWordStepHandler;
            _parser.OnProcessCanceled += OnProcessCanceledHandler;
            _parser.OnProcessAudioDownload += OnProcessAudioDownloadHandler;
            _parser.OnProcesAudioDownloadOk += OnProccessAudioDownloadOk;
        }

        private void OnProccessAudioDownloadOk()
        {    
            this.Dispatcher.Invoke(() => {
                var count = Int32.Parse(downloadAudioCounterTextBlock.Text);
                count++;
                downloadAudioCounterTextBlock.Text = count.ToString();
            });
        }

        private void OnProcessAudioDownloadHandler(string service)
        {
            this.Dispatcher.Invoke(() => {
                processDownloadAudioStatusTextBlock.Text = service;
            });
        }

        private void InitSettings()
        {
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
            AppSettings.Instance.EveryIterSave = Int32.Parse(StepSave.Text);
            AppSettings.Instance.SheetIndex = Int32.Parse(SheetNumber.Text);
            AppSettings.Instance.StartNumberIndex = Int32.Parse(startNumberIndex.Text);
            AppSettings.Instance.EndNumberIndex = Int32.Parse(endNumberIndex.Text);
            AppSettings.Instance.PicSize = Int32.Parse(picSize.Text);
        }

        private void OnProcessCanceledHandler()
        {
            this.Dispatcher.Invoke(() => {
                progress.Value = 0;
                startButton.IsEnabled = true;
                cancelButton.IsEnabled = false;
                processStatusTextBlock.Text = "Отменено";
            });
        }

        private void OnProcessWordStepHandler(string word)
        {
            this.Dispatcher.Invoke(() => {
                processDownloadAudioStatusTextBlock.Text = "";
                processStatusTextBlock.Text = word;
            });
        }

        private void OnProcessIndexStepHandler(int index)
        {
            this.Dispatcher.Invoke(() => {
                indexCurrentProcessTextBox.Text = index.ToString();
                progress.Value = (index * 100) / AppSettings.Instance.EndNumberIndex;
            });
        }

        private void OnProcessCompletedHandler()
        {
            this.Dispatcher.Invoke(() =>
            {
                startButton.IsEnabled = true;
                cancelButton.IsEnabled = false;
                processStatusTextBlock.Text = "Процесс завершен";
                processStatusTextBlock.Foreground = new SolidColorBrush(Colors.Orange);
                progress.Value = 100;
            });
        }

        private void OnProcessStartedHandler()
        {
            this.Dispatcher.Invoke(()=> 
            {
                startButton.IsEnabled = false;
                cancelButton.IsEnabled = true;
                processStatusTextBlock.Text = "Процесс запущен";
                processStatusTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                progress.Value = 0;
            });
        }

        private void ColumnSettings_TextChanged(object sender, TextChangedEventArgs e)
        {      
            TextBox textBox = (TextBox)sender;
            var match = Regex.Match(textBox.Text, "[aA-zZ]");
            
            if (textBox.Text.Length > 0)
            {
                if (!match.Success)
                {
                    textBox.Text = "A";
                    MessageBox.Show("Значение должно быть в диапозоне A-Z", "Ошибка ввода");
                }
                if (textBox.Text.Length > 1)
                {
                    textBox.Text = "A";
                    MessageBox.Show("Можно ввести только одну букву", "Ошибка ввода");
                }
            }   
        }

        private void OnPictureCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadPicture = true;
        }

        private void OnPictureCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadPicture = false;
        }

        private void OnAudioTurengChecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadAudioTureng = true;
        }

        private void OnAudioTurengUnchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadAudioTureng = false;
        }

        private void OnAudioForvoChecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadAudioForvo = true;
        }

        private void OnAudioForvoUnchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadAudioForvo = false;
        }

        private void OnAudioYahooChecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadAudioYahoo = true;
        }

        private void OnAudioYahooUnchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadAudioYahoo = false;
        }

        private void OnAudioDictionaryChecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadAudioDictionary = true;
        }

        private void OnAudioDictionaryUnchecked(object sender, RoutedEventArgs e)
        {
            AppSettings.Instance.DownloadAudioDictionary = false;
        }
    }
}
