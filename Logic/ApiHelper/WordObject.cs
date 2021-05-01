using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.ApiHelper
{
    /// <summary>
    /// A data transfer object for retrieving the definition of a word from a public API.
    /// </summary>
    public class WordObject
    {
        public string Word { get; set; }
        public List<DefinitionObject> Definitions { get; set; }
    }
}