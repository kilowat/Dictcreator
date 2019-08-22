using Dictcreator.Core.Fetchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace Dictcreator.Core
{
    public class Parser
    {
        public event ProcessStarted OnProcessStared;
        public event ProcessCompleted OnProcessCompleted;
        public event ProcessIndexStep OnProcessIndexStep;
        public event ProcessWordStep OnProcessWordStep;
        public event ProcessCanceled OnProcessCanceled;

        private List<DataFetcher> _dataFetcher;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancelToken;

        public CancellationToken CancelToken { get => _cancelToken; private set => _cancelToken = value; }
        public CancellationTokenSource TokenSource { get => _tokenSource; private set => _tokenSource = value; }

        public Parser()
        {
            InitParser();
        }

        private void InitParser()
        {
            _dataFetcher = new List<DataFetcher>();

            _dataFetcher.Add(new TranscriptionFetcherTopPhonetics());
            _dataFetcher.Add(new TranslateFetcherReverso());
            _dataFetcher.Add(new ExamplesFetcherReverso());
            _dataFetcher.Add(new AudioFetcherWordHunt());
            _dataFetcher.Add(new LinkFetcherMerriemWebster());
            _dataFetcher.Add(new LinkFetcherReverso());
            _dataFetcher.Add(new LinkFetcherWordHunt());
            _dataFetcher.Add(new LinkFetcherYouglish());

            TokenSource = new CancellationTokenSource();
            CancelToken = TokenSource.Token;
        }

        public async Task<bool> RunAsync()
        {
            if (CheckSettings())
            {
                try
                {
                    await Task.Run(() => Run(), CancelToken);
                }
                catch (Exception e)
                {
                    if (e is OperationCanceledException)
                    {
                        if (OnProcessCanceled != null)
                        {
                            OnProcessCanceled();
                        }

                        MessageBox.Show("Операция отменена пользователем", "Работа приложения");
                    }
                }
                finally
                {
                    TokenSource.Dispose();
                }
            }
            else
            {
                return false;
            }

            return true;
        }
        public void Run()
        {
            ReadWriteFile();
        }

        private bool CheckSettings()
        {
            return true;
        }

        private void ReadWriteFile()
        {
            if (OnProcessStared != null)
            {
                OnProcessStared();
            }

            CancelToken.ThrowIfCancellationRequested();

            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(AppSettings.Instance.FileXlsPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            for (int i = AppSettings.Instance.StartNumberIndex; i <= AppSettings.Instance.EndNumberIndex; i++)
            {
                var colWordIndex = AppSettings.Instance.ColumnNameIndexMap[ColumnName.WORD];

                var currentWord = xlRange.Cells[i, colWordIndex].Value2.ToString();

                if (OnProcessIndexStep != null)
                {
                    OnProcessIndexStep(i);
                }
                if (OnProcessWordStep != null)
                {
                    OnProcessWordStep(currentWord);
                }

                //Thread.Sleep(1000);

                if (CancelToken.IsCancellationRequested)
                {
                    CancelToken.ThrowIfCancellationRequested();
                }

                WriteIndex(i);

                foreach (DataFetcher fetcher in _dataFetcher)
                {
                    string result = fetcher.GetResult(currentWord);

                    if (fetcher.CellExlType == CellType.STRING)
                    {

                    }
                    else if (fetcher.CellExlType == CellType.LINK)
                    {

                    }
                }
            }

            if (OnProcessCompleted != null)
            {
                OnProcessCompleted();
            }
        }

        private void WriteIndex(int index)
        {

        }
    }

    public delegate void ProcessStarted();
    public delegate void ProcessCanceled();
    public delegate void ProcessCompleted();
    public delegate void ProcessIndexStep(int index);
    public delegate void ProcessWordStep(string word);
}
