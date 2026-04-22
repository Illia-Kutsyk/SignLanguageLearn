using System;

namespace SignLanguageLearn.Models
{
    public class Achievement
    {
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public DateTime Date { get; set; }
        public double Score { get; set; }
    }
}