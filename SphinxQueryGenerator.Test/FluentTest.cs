using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SphinxQueryGenerator.Test
{
    [TestClass]
    public class FluentTest
    {
        [TestMethod]
        public void FluentQueryGenerateTest()
        {
            var criterion = new PersonFindCriterion();
            criterion.PageNum = 1;
            criterion.PageSize = 20;
            criterion.Gender = GenderType.Female;
            criterion.CityId = 100;
            criterion.Name = "Ivanov Ivan";

            var queryGenerator = new SphinxQueryGenerator("Persons");
            var query = queryGenerator
                .AsFluent()
                .Match("@(FirstName,LastName)", criterion.Name, SphinxFluentQuery.IsNotNull)
                .Match("Company", null, SphinxFluentQuery.IsNotNull)
                .Key("GenderId", "=", criterion.Gender, SphinxFluentQuery.IsNotNull)
                .Key("CityId", "=", criterion.CityId, SphinxFluentQuery.IsNotNull)
                .Limit(criterion.Offset, criterion.PageSize)
                .Generate();
            
            Assert.AreEqual(@"SELECT ID FROM Persons  WHERE  MATCH('@(FirstName,LastName) Ivanov Ivan') " +
                "AND GenderId = 2 AND CityId = 100 LIMIT 0, 20;show meta;", 
                query);
        }
    }
}
