using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core
{
    public class TranscriptionFetcher : DataFetcher
    {
        protected override ColumnName ColName => ColumnName.TRANSCRIPTION;

        public override string GetResult(string word)
        {
            return word;
        }
    }
}
