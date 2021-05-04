using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.ApiHelper
{
    /// <summary>
    /// Contains a single definition and its associated part of speech.
    /// </summary>
    public class DefinitionObject
    {
        public string Definition { get; set; }
        public string PartOfSpeech { get; set; }
    }
}