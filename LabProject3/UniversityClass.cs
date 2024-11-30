using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LabProject3
{
    public class UniversityClass
    {
        public string Subject { get; set; }
        public string GroupNumber { get; set; }
        public string Specialization { get; set; }
        public string Lecturer { get; set; }
        public int Course { get; set; }
        public List<string> Schedule { get; set; }

        [JsonIgnore]
        public string FormattedSchedule => Schedule != null ? string.Join("\n", Schedule) : string.Empty;

        public UniversityClass(string subject, List<string> schedule, string groupNumber, string specialization, int course, string lecturer)
        {
            Subject = subject;
            Schedule = schedule;
            GroupNumber = groupNumber;
            Specialization = specialization;
            Course = course;
            Lecturer = lecturer;
        }
    }
}
