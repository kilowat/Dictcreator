using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dictcreator
{
    public class AppSettings
    {
        #region Properties
        private static AppSettings _instance;

        private Dictionary<ColumnName, int> _columnNameIndexMap = new Dictionary<ColumnName, int>();
        private Dictionary<string, int> _colCharIndexMap = new Dictionary<string, int>();
        private Dictionary<int, string> _colIndexCharMap = new Dictionary<int, string>();
        private string _audioDirName = "audio";
        private string _pictureDirName = "picture";

        private string _fileXlsPath = string.Empty;
        private string _colNumberIndex = string.Empty;
        private string _colEnWord = string.Empty;
        private string _colTranscript = string.Empty;
        private string _colTranslation = string.Empty;
        private string _colExamples = string.Empty;
        private int _maxExample = 0;
        private string _colAudio = string.Empty;
        private string _colYouglish = string.Empty;
        private string _colContextReverso = string.Empty;
        private string _colWoordHunt = string.Empty;
        private string _colMerWebster = string.Empty;
        private int _startNumberIndex = 1;
        private int _endNumberIndex = 100;
        private string _pathToAudio = string.Empty;
        private string _pathToPicture = string.Empty;
        private int _everyIterSave = 10;
        private int _sheetIndex = 1;
        private bool _downloadAudioTureng = true;
        private bool _downloadAudioForvo = true;
        private bool _downloadAudioVocabulary = true;
        private bool _downloadAudioDictionary = true;
        private bool _downloadPicture;
        private int _picSize = 400;
        public static AppSettings Instance
        {
            private set { }
            get
            {
                if (_instance == null)
                {
                    _instance = new AppSettings();
                }
                return _instance;
            }
        }

        public string FileXlsPath { get => _fileXlsPath; set => _fileXlsPath = value; }
        public string ColNumberIndex
        {
            get => _colNumberIndex;
            set
            {
                if(value.Length == 0) value = "EMPTY";

                _colNumberIndex = value;
                ColumnNameIndexMap[ColumnName.NUMBER] = ColCharIndexMap[value];
            }
        }
        public string ColEnWord
        {
            get => _colEnWord;
            set
            {
                _colEnWord = value;
                ColumnNameIndexMap[ColumnName.WORD] = ColCharIndexMap[value];
            }
        }
        public string ColTranscript
        {
            get => _colTranscript;
            set
            {
                if (value.Length == 0) value = "EMPTY";

                _colTranscript = value;
                ColumnNameIndexMap[ColumnName.TRANSCRIPTION] = ColCharIndexMap[value];
            }
        }
        public string ColTranslation
        {
            get => _colTranslation;
            set
            {
                if (value.Length == 0) value = "EMPTY";

                _colTranslation = value;
                ColumnNameIndexMap[ColumnName.TRANSLATE] = ColCharIndexMap[value];
            }
        }
        public string ColExamples
        {
            get => _colExamples;
            set
            {
                if (value.Length == 0) value = "EMPTY";

                _colExamples = value;
                ColumnNameIndexMap[ColumnName.EXAMPLES] = ColCharIndexMap[value];
            }
        }
        public int MaxExample { get => _maxExample; set => _maxExample = value; }
        public string ColAudio
        {
            get => _colAudio;
            set
            {
                if (value.Length == 0) value = "EMPTY";

                _colAudio = value;
                ColumnNameIndexMap[ColumnName.AUDIO] = ColCharIndexMap[value];
            }
        }
        public string ColYouglish
        {
            get => _colYouglish;
            set
            {
                if (value.Length == 0) value = "EMPTY";
                _colYouglish = value;
                ColumnNameIndexMap[ColumnName.YOUGLISH] = ColCharIndexMap[value];
            }
        }
        public string ColContextReverso
        {
            get => _colContextReverso;
            set
            {
                if (value.Length == 0) value = "EMPTY";

                _colContextReverso = value;
                ColumnNameIndexMap[ColumnName.REVERSO] = ColCharIndexMap[value];
            }
        }
        public string ColWoordHunt
        {
            get => _colWoordHunt;
            set
            {
                if (value.Length == 0) value = "EMPTY";

                _colWoordHunt = value;
                ColumnNameIndexMap[ColumnName.WORD_HUNT] = ColCharIndexMap[value];
            }
        }
        public string ColMerWebster
        {
            get => _colMerWebster;
            set
            {
                if (value.Length == 0) value = "EMPTY";

                _colMerWebster = value;
                ColumnNameIndexMap[ColumnName.MERRIAM_WEBSTER] = ColCharIndexMap[value];
            }
        }
        public int StartNumberIndex
        {
            get => _startNumberIndex; 
            set
            {
                if (value < 1)
                {
                    value = 1;
                }
                _startNumberIndex = value;
            }
        }

        public int EndNumberIndex { get => _endNumberIndex; set => _endNumberIndex = value; }
        public Dictionary<string, int> ColCharIndexMap { get => _colCharIndexMap; private set => _colCharIndexMap = value; }
        public Dictionary<ColumnName, int> ColumnNameIndexMap { get => _columnNameIndexMap; private set => _columnNameIndexMap = value; }
        public string PathToAudio
        {
            get
            {
                if (FileXlsPath.Length > 0)
                {
                    var arr = FileXlsPath.Split('\\').ToList();
                    arr.RemoveAt(arr.Count - 1);
                    _pathToAudio = string.Join("\\", arr);
                    _pathToAudio += "\\"+ AudioDirName;

                    try
                    {
                        Directory.CreateDirectory(_pathToAudio);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        MessageBox.Show("Внимание у приложения нет прав на создание раздела");
                    }
                }

                return  _pathToAudio;
            } 
            private set => _pathToAudio = value;
        }

        public string AudioDirName { get => _audioDirName; private set => _audioDirName = value; }
        public int EveryIterSave
        {
            get
            {
                return _everyIterSave;
            }
            set
            {
                _everyIterSave = value;
            }
        }

        public int SheetIndex
        {
            get
            {
                return _sheetIndex;
            }
            set
            {
                _sheetIndex = value;
            }
        }

        public Dictionary<int, string> ColIndexCharMap { get => _colIndexCharMap; private set => _colIndexCharMap = value; }
        public string PictureDirName { get => _pictureDirName; set => _pictureDirName = value; }
        public string PathToPicture
        {
            get
            {
                if (FileXlsPath.Length > 0)
                {
                    var arr = FileXlsPath.Split('\\').ToList();
                    arr.RemoveAt(arr.Count - 1);
                    _pathToPicture = string.Join("\\", arr);
                    _pathToPicture += "\\" + PictureDirName;

                    try
                    {
                        Directory.CreateDirectory(_pathToPicture);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        MessageBox.Show("Внимание у приложения нет прав на создание раздела");
                    }
                }

                return _pathToPicture;
            }
        }

        public bool DownloadPicture { get => _downloadPicture; set => _downloadPicture = value; }
        public int PicSize { get => _picSize; set => _picSize = value; }
        public bool DownloadAudioTureng { get => _downloadAudioTureng; set => _downloadAudioTureng = value; }
        public bool DownloadAudioForvo { get => _downloadAudioForvo; set => _downloadAudioForvo = value; }
        public bool DownloadAudioVocabulary { get => _downloadAudioVocabulary; set => _downloadAudioVocabulary = value; }
        public bool DownloadAudioDictionary { get => _downloadAudioDictionary; set => _downloadAudioDictionary = value; }

        #endregion

        private AppSettings()
        {
            InitColCharIndexMap();

            ColumnNameIndexMap.Add(ColumnName.EMPTY, -1);
        }

        private void InitColCharIndexMap()
        {
            int i = 1;

            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                ColCharIndexMap[letter.ToString()] = i;
                ColIndexCharMap[i] = letter.ToString();
                i++;
            }

            //Empty col index
            ColCharIndexMap["EMPTY"] = -1;
            ColIndexCharMap[-1] = "EMPTY";
        }
    }

    public enum ColumnName
    {
        NUMBER,
        WORD,
        TRANSCRIPTION,
        TRANSLATE,
        EXAMPLES,
        AUDIO,
        YOUGLISH,
        REVERSO,
        WORD_HUNT,
        MERRIAM_WEBSTER,
        EMPTY,
        PICTURE,
    }
}
