using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SphinxQueryGenerator.Test
{
    [TestClass]
    public class AttributeTest
    {
        [TestMethod]
        public void AttributeQueryGenerateTest()
        {
            var criterion = new PersonFindCriterion();
            criterion.PageNum = 1;
            criterion.PageSize = 20;
            criterion.Gender = GenderType.Female;
            criterion.CityId = 100;
            criterion.Name = "Ivanov Ivan";
            var queryGenerator = new SphinxQueryGenerator("Persons");
            var query = queryGenerator.GenerateQuery<PersonFindCriterion>(criterion);
            Assert.AreEqual(@"SELECT id FROM Persons  WHERE MATCH('@(FirstName,LastName) Ivanov Ivan') AND GenderId=2 AND CityId=100  limit 0, 20; show meta;", 
                query);
        }
    }
}
