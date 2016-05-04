# Generate query from criterion object to sphinx

Library exists two methods query generation

- Attribute based on criterion objects
- Fluent api

## Attribute based method

define criterion class with special attribute

```
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
```

generate query invoke generation method

```
var criterion = new PersonFindCriterion();
criterion.PageNum = 1;
criterion.PageSize = 20;
criterion.Gender = GenderType.Female;
criterion.CityId = 100;
criterion.Name = "Ivanov Ivan";
var queryGenerator = new SphinxQueryGenerator("Persons");
var query = queryGenerator.GenerateQuery<PersonFindCriterion>(criterion);
```

## Fluent api

```
var queryGenerator = new SphinxQueryGenerator("Persons");
var query = queryGenerator
	.AsFluent()
	.Match("@(FirstName,LastName)", criterion.Name, SphinxFluentQuery.IsNotNull)
	.Match("Company", null, SphinxFluentQuery.IsNotNull)
	.Key("GenderId", "=", criterion.Gender, SphinxFluentQuery.IsNotNull)
	.Key("CityId", "=", criterion.CityId, SphinxFluentQuery.IsNotNull)
	.Limit(criterion.Offset, criterion.PageSize)
	.Generate();
```