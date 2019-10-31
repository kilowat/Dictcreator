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
        public event ProccessAudioDownload OnProcessAudioDownload;
        public event ProccessAudioDownloadOk OnProcesAudioDownloadOk;

        public CancellationToken CancelToken { get => _cancelToken; private set => _cancelToken = value; }
        public CancellationTokenSource TokenSource { get => _tokenSource; private set => _tokenSource = value; }
        public bool IsWorked { get => _isWorked; private set => _isWorked = value; }

        private AudioFetcherForvo _forvoFetcher = new AudioFetcherForvo();
        private AudioFetcherTuren _turenFetcher = new AudioFetcherTuren();
        private AudioFethcerVocabulary _vocabularyFetcher = new AudioFethcerVocabulary();
        private AudioFetcherDictionary _dictonaryFetcher = new AudioFetcherDictionary();

        private List<DataFetcher> _audioFetchers;

        public Parser()
        {
            
        }
        public void InitParser()
        {   
            _dataFetcher = new List<DataFetcher>();
            
            _dataFetcher.Add(new TranscriptionFetcherWordHunt());
            _dataFetcher.Add(new TranslateFetcherWordHunt());
            _dataFetcher.Add(new ExamplesFetcherWoordHunt());
            _dataFetcher.Add(new AudioFetcherWordHunt());
            _dataFetcher.Add(new LinkFetcherMerriemWebster());
            _dataFetcher.Add(new LinkFetcherReverso());
            _dataFetcher.Add(new LinkFetcherWordHunt());
            _dataFetcher.Add(new LinkFetcherYouglish());
            _dataFetcher.Add(new PictureFetcher());

            _audioFetchers = new List<DataFetcher>();

            if (AppSettings.Instance.DownloadAudioVocabulary)
                _audioFetchers.Add(new AudioFethcerVocabulary());

            if (AppSettings.Instance.DownloadAudioDictionary)
                _audioFetchers.Add(new AudioFetcherDictionary());

            if (AppSettings.Instance.DownloadAudioTureng)
                _audioFetchers.Add(new AudioFetcherTuren());

            if (AppSettings.Instance.DownloadAudioForvo)
                _audioFetchers.Add(new AudioFetcherForvo());
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
            OnProcessStared?.Invoke();

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

                OnProcessIndexStep?.Invoke(i);
                OnProcessWordStep?.Invoke(currentWord);

                //Thread.Sleep(1000);

                if (CancelToken.IsCancellationRequested)
                {
                    //CancelToken.ThrowIfCancellationRequested();
                }

                WriteIndex(i);

                foreach (DataFetcher fetcher in _dataFetcher)
                {
                    if (fetcher.CellExlType != CellType.EMPTY)
                    {
                        if (fetcher.ColIndex == -1) continue;
                        if (i % AppSettings.Instance.EveryIterSave == 1) _xlWorkbook.Save();
                    }

                    string result = "";
                    result = fetcher.GetResult(currentWord);

                    if (fetcher.ColIndex == AppSettings.Instance.ColumnNameIndexMap[ColumnName.AUDIO] && fetcher.ColIndex != -1)
                    {
                        OnProcessAudioDownload?.Invoke(fetcher.ServiceName);

                        foreach (var audioFetcher in _audioFetchers)
                        {
                            if (result == String.Empty)
                            {
                                OnProcessAudioDownload?.Invoke(audioFetcher.ServiceName);
                                result = audioFetcher.GetResult(currentWord);
                            }
                        }
                        if (result == String.Empty)
                        {
                            OnProcessAudioDownload?.Invoke("НЕ НАШЛИ!");
                        }
                        else
                        {
                            OnProcesAudioDownloadOk?.Invoke();
                        }
                    }

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
    public delegate void ProccessAudioDownload(string service);
    public delegate void ProccessAudioDownloadOk();
}
