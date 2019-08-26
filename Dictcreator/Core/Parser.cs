using Dictcreator.Core.Fetchers;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace Dictcreator.Core
{
    public class Parser
    {
        private List<DataFetcher> _dataFetcher;
        private CancellationTokenSource _tokenSource;
        private CancellationToken _cancelToken;

        private Excel.Application _xlApp;
        private Excel.Workbook _xlWorkbook;
        private Excel._Worksheet _xlWorksheet;
        private Excel.Range _xlRange;

        private bool _isWorked;

        public event ProcessStarted OnProcessStared;
        public event ProcessCompleted OnProcessCompleted;
        public event ProcessIndexStep OnProcessIndexStep;
        public event ProcessWordStep OnProcessWordStep;
        public event ProcessCanceled OnProcessCanceled;

        public CancellationToken CancelToken { get => _cancelToken; private set => _cancelToken = value; }
        public CancellationTokenSource TokenSource { get => _tokenSource; private set => _tokenSource = value; }
        public bool IsWorked { get => _isWorked; private set => _isWorked = value; }

        public Parser()
        {
            InitParser();
        }


        private void InitParser()
        {
            _dataFetcher = new List<DataFetcher>();

            _dataFetcher.Add(new TranscriptionFetcherWordHunt());
            _dataFetcher.Add(new TranslateFetcherReverso());
            _dataFetcher.Add(new ExamplesFetcherWoordHunt());
            _dataFetcher.Add(new AudioFetcherWordHunt());
            _dataFetcher.Add(new LinkFetcherMerriemWebster());
            _dataFetcher.Add(new LinkFetcherReverso());
            _dataFetcher.Add(new LinkFetcherWordHunt());
            _dataFetcher.Add(new LinkFetcherYouglish());
        }

        public async Task<bool> RunAsync()
        {
            if (CheckSettings())
            {
                try
                {
                    TokenSource = new CancellationTokenSource();
                    CancelToken = TokenSource.Token;
                    IsWorked = true;
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
                    }
                    else
                    {
                        MessageBox.Show(e.Message, "Произошла обшика");

                        if (OnProcessCompleted != null)
                            OnProcessCompleted();
                    }
                }
                finally
                {
                    StopOperation();
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

        public void CancelOperation()
        {
            if(IsWorked)
                TokenSource.Cancel();
        }

        public void StopOperation()
        {
            TokenSource.Dispose();
            IsWorked = false;
            CloseAndSaveExcelFile();
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

            _xlApp = new Excel.Application();
            _xlWorkbook = _xlApp.Workbooks.Open(AppSettings.Instance.FileXlsPath);

            _xlWorksheet = _xlWorkbook.Sheets[AppSettings.Instance.SheetIndex];
            _xlRange = _xlWorksheet.UsedRange;

            var number = AppSettings.Instance.StartNumberIndex - 1;

            for (int i = AppSettings.Instance.StartNumberIndex; i <= AppSettings.Instance.EndNumberIndex; i++)
            {
                number++;

                var curCell = (Excel.Range)_xlWorksheet.get_Range(AppSettings.Instance.ColEnWord + i);

                if (curCell.Value2 == null) continue;

                var currentWord = curCell.Value2;

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
                    if (fetcher.ColIndex == -1) continue;

                    if (i % AppSettings.Instance.EveryIterSave == 1) _xlWorkbook.Save();

                    string result = fetcher.GetResult(currentWord);

                    if (fetcher.CellExlType == CellType.STRING)
                    {
                        _xlWorksheet.Cells[i, fetcher.ColIndex] = result;
                    }
                    else if (fetcher.CellExlType == CellType.LINK)
                    {
                        var fCell = (Excel.Range)_xlWorksheet.get_Range(AppSettings.Instance.ColIndexCharMap[fetcher.ColIndex] + i);
                       _xlWorksheet.Hyperlinks.Add(fCell, result, Type.Missing, fetcher.ServiceName+"/" + currentWord, fetcher.ServiceName+"/" + currentWord);

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
            if(AppSettings.Instance.ColumnNameIndexMap[ColumnName.NUMBER] > 0)
                _xlWorksheet.Cells[index, AppSettings.Instance.ColumnNameIndexMap[ColumnName.NUMBER]] = index.ToString();
        }

        private void CloseAndSaveExcelFile()
        {
            if (_xlWorkbook != null)
                _xlWorkbook.Save();

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(_xlRange);
            Marshal.ReleaseComObject(_xlWorksheet);

            //close and release
            _xlWorkbook.Close();
            Marshal.ReleaseComObject(_xlWorkbook);

            //quit and release
            _xlApp.Quit();
            Marshal.ReleaseComObject(_xlApp);
        }
    }

    public delegate void ProcessStarted();
    public delegate void ProcessCanceled();
    public delegate void ProcessCompleted();
    public delegate void ProcessIndexStep(int index);
    public delegate void ProcessWordStep(string word);
}
