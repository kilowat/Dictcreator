using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictcreator.Core
{
    public interface IParserStrategy
    {
        Dictionary<int, string> Result(string word);
    }
}
