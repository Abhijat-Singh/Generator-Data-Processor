using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorsDataProcessor.Models
{
	public class AppSettings
	{
        public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public string ReferenceDataFilePath { get; set; }
    }
}
