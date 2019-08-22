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

        private string _audioDirName = "audio";

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
                _colTranscript = value;
                ColumnNameIndexMap[ColumnName.TRANSCRIPTION] = ColCharIndexMap[value];
            }
        }
        public string ColTranslation { get => _colTranslation; set => _colTranslation = value; }
        public string ColExamples { get => _colExamples; set => _colExamples = value; }
        public int MaxExample { get => _maxExample; set => _maxExample = value; }
        public string ColAudio { get => _colAudio; set => _colAudio = value; }
        public string ColYouglish { get => _colYouglish; set => _colYouglish = value; }
        public string ColContextReverso { get => _colContextReverso; set => _colContextReverso = value; }
        public string ColWoordHunt { get => _colWoordHunt; set => _colWoordHunt = value; }
        public string ColMerWebster { get => _colMerWebster; set => _colMerWebster = value; }
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

        #endregion

        private AppSettings()
        {
            InitColCharIndexMap();
        }

        private void InitColCharIndexMap()
        {
            int i = 0;
            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                ColCharIndexMap[letter.ToString()] = i;
                i++;
            }
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
        MERRIAM_WEBSTER
    }
}
