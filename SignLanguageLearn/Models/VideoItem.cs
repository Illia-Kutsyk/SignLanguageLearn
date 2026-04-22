using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignLanguageLearn.Models
{
    public class VideoItem
    {
        public string Title { get; set; }
        public string FileName { get; set; } // Додано
        public string Category { get; set; } // Додано
        public string Language { get; set; }
    }
}