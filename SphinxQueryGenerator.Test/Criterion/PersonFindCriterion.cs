using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphinxQueryGenerator.Test
{
    /// <summary>
    /// Параметры поиска персон
    /// </summary>
    public class PersonFindCriterion : PagedCriterion
    {
        [SphinxMatch(fieldName: "@(FirstName,LastName)")]
        public string Name { get; set; }
        [SphinxId("GenderId", nullValues: new int[] { 0 })]
        public GenderType? Gender { get; set;}
        public int? AgeFrom { get; set; }
        [SphinxId("BirthDate", comparison: ComparisonType.Le)]
        [SphinxConstant("BirthDate", 0, ComparisonType.NotEq)]
        public int? AgeFromSphinx
        {
            get
            {
                if (AgeFrom == null) return null;
                var birtdayFrom = DateTime.Now.AddYears(-AgeFrom.Value);
                return Convert.ToInt32(string.Format("{0}{1:00}{2:00}", birtdayFrom.Year, birtdayFrom.Month, birtdayFrom.Day));
            }
        }

        [SphinxId("BirthDate", comparison: ComparisonType.Ge)]
        public int? AgeToSphinx
        {
            get
            {
                if (AgeTo == null) return null;
                var birtdayTo = DateTime.Now.AddYears(-AgeTo.Value);
                return Convert.ToInt32(string.Format("{0}{1:00}{2:00}", birtdayTo.Year, birtdayTo.Month, birtdayTo.Day));
            }
        }

        public int? AgeTo { get; set; }
        
        public string HomeTown { get; set; }
        [SphinxId("CityId")]
        public int? CityId { get; set; }
        
    }
}
