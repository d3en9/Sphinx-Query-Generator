using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SphinxQueryGenerator
{
    public partial class SphinxQueryGenerator
    {
        private readonly IQueryEscaper _escaper;
        private readonly string _indexName;
        public SphinxQueryGenerator(string indexName)
        {
            _indexName = indexName;
            _escaper = new SphinxQueryEscaper();
        }

        /// <summary>
        /// Сгенерировать запрос к sphinx на основе аттрибутов и значений свойств указанных criterion
        /// </summary>
        /// <param name="criterion"></param>
        /// <returns></returns>
        public string GenerateQuery<T>(T criterion) where T : ICriterion
        {
            // поиск свойств с установленными атрибутами для сфинкса заполнение списков атрибутов
            var properties = criterion.GetType().GetProperties();
            var matchAttributes = new List<CriterionPropertyMeta<SphinxMatchAttribute, T>>();
            var idAttributes = new List<CriterionPropertyMeta<SphinxIdAttribute, T>>();
            var constAttributes = new List<CriterionPropertyMeta<SphinxConstantAttribute, T>>();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes<SphinxAttribute>();
                foreach(var attribute in attributes)
                {
                    if (attribute is SphinxMatchAttribute)
                    {
                        matchAttributes.Add(new CriterionPropertyMeta<SphinxMatchAttribute, T>((SphinxMatchAttribute)attribute, property, criterion));
                    }
                    else if (attribute is SphinxIdAttribute)
                    {
                        idAttributes.Add(new CriterionPropertyMeta<SphinxIdAttribute, T>((SphinxIdAttribute)attribute, property, criterion));
                    }
                    else if (attribute is SphinxConstantAttribute)
                    {
                        constAttributes.Add(new CriterionPropertyMeta<SphinxConstantAttribute, T>(
                            (SphinxConstantAttribute)attribute, property, criterion));
                    }
                }
                
            }

            string match = GenerateMatch(matchAttributes);
            string ids = GenerateIds(idAttributes);
            string constant = GenerateConstant(constAttributes);
            string[] whereArray = { match, ids, constant };
            var where = string.Join(" AND ", whereArray.Where(_ => _ != null));
            if (where != "")
            {
                where = $" WHERE {@where}";
            }
            const string limit = " limit @offset, @pageSize;";
            const string showMeta = "show meta;";
            var query = "SELECT id FROM " + _indexName  + where + limit + showMeta;
            return query;
        }

        private string GenerateMatch<T>(IEnumerable<CriterionPropertyMeta<SphinxMatchAttribute, T>> matches) where T : ICriterion
        {
            var matchQueries = (from match in matches
                                let queryValue = match.Property.GetValue(match.Criterion)
                                where queryValue != null
                                select
                                    $"{match.Attribute.FieldName} {_escaper.Escape(queryValue.ToString())}")
                               .ToList();
            if (matchQueries.Count > 0)
            {
                return $"MATCH('{string.Join(" ", matchQueries)}')";
            }
            else return null;
        }

        private string GenerateIds<T>(IEnumerable<CriterionPropertyMeta<SphinxIdAttribute, T>> ids) where T : ICriterion
        {
            List<string> idQueries = new List<string>();
            foreach (var id in ids)
            {
                var queryValue = id.Property.GetValue(id.Criterion);
                if (queryValue != null && (id.Attribute.NullValues == null || !id.Attribute.NullValues.Contains((int)queryValue)))
                {
                    var idQuery = string.Format("{0}{1}{2}", id.Attribute.FieldName, id.Attribute.Comparison.GetDescription(), (int)queryValue);
                    idQueries.Add(idQuery);
                }
            }
            if (idQueries.Count > 0)
            {
                return string.Join(" AND ", idQueries);
            }
            else return null;
        }

        private string GenerateConstant<T>(IEnumerable<CriterionPropertyMeta<SphinxConstantAttribute, T>> constants) where T : ICriterion
        {
            List<string> constantQueries = new List<string>();
            foreach (var constant in constants)
            {
                var queryValue = constant.Property.GetValue(constant.Criterion);
                if (queryValue != null)
                {
                    var constantQuery = string.Format("{0}{1}{2}", constant.Attribute.FieldName, 
                        constant.Attribute.Comparison.GetDescription(), constant.Attribute.Value);
                    constantQueries.Add(constantQuery);
                }
            }
            if (constantQueries.Count > 0)
            {
                return string.Join(" AND ", constantQueries);
            }
            else return null;
        }

        public class CriterionPropertyMeta<A, C> where A : SphinxAttribute where C : ICriterion
        {
            public A Attribute { get; set; }
            public PropertyInfo Property { get; set; }
            public C Criterion { get; set; }

            public CriterionPropertyMeta(A attribute, PropertyInfo property, C criterion)
            {
                Attribute = attribute;
                Property = property;
                Criterion = criterion;
            }
        }
    }
}
