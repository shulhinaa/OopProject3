using System;
using System.Collections.Generic;
using System.Linq;

namespace LabProject3
{
    public class LINQSearch
    {
        private bool MatchesKeyword(string propertyValue, string keyword)
        {
            return !string.IsNullOrEmpty(propertyValue) &&
                   propertyValue.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchesSchedule(List<string> schedule, string keyword)
        {
            return schedule != null && schedule.Any(day => day.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        public List<UniversityClass> Search(List<string> criterias, List<UniversityClass> classes)
        {
            if (criterias == null || criterias.Count == 0)
            {
                return new List<UniversityClass>(); 
            }

            int courseCriteria = 0;
            var numericCriteria = criterias.FirstOrDefault(c => int.TryParse(c, out courseCriteria));

            var results = (from uniClass in classes
                           where criterias.All(keyword =>
                               MatchesKeyword(uniClass.Subject, keyword) ||
                               MatchesKeyword(uniClass.Lecturer, keyword) ||
                               MatchesKeyword(uniClass.GroupNumber, keyword) ||
                               MatchesKeyword(uniClass.Specialization, keyword) ||
                               MatchesSchedule(uniClass.Schedule, keyword)) &&
                               (numericCriteria == null || uniClass.Course == courseCriteria) 
                           select uniClass).ToList();

            return results;
        }
    }
}

