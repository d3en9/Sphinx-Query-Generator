using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace SphinxQueryGenerator
{
    public partial class SphinxQueryGenerator
    {

        public SphinxFluentQuery AsFluent()
        {
            return new SphinxFluentQuery(this._indexName);        
        }

    }

    public class SphinxFluentQuery
    {
        protected List<SphinxFluentQuery> _queries = new List<SphinxFluentQuery>();
        public static Func<object, bool> IsNull = (o) => string.IsNullOrWhiteSpace(o?.ToString());
        public static Func<object, bool> IsNotNull = (o) => !string.IsNullOrWhiteSpace(o?.ToString());

        public SphinxFluentQuery()
        {

        }

        public SphinxFluentQuery(string indexName)
        {
            _queries.Add(new SphinxFluentQuerySelect(indexName));
        }


        public SphinxFluentQuery Match(string fieldName, object value, Func<object, bool> predicate)
        {
            _queries.Add(new SphinxFluentQueryMatch(fieldName, value, predicate));
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="comparator">Операция сравнения = ...</param>
        /// <param name="value"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public SphinxFluentQuery Key(string fieldName, string comparator, object value, Func<object, bool> predicate)
        {
            _queries.Add(new SphinxFluentQueryKey(fieldName, comparator, value, predicate));
            return this;
        }

        public SphinxFluentQuery Limit(long offset, int count)
        {
            _queries.Add(new SphinxFluentQueryLimit(offset, count));
            return this;
        }

        public string Generate()
        {
            var selectQuery = _queries.OfType<SphinxFluentQuerySelect>().Single();
            var matchQueries = _queries.OfType<SphinxFluentQueryMatch>();
            var keyQueries = _queries.OfType<SphinxFluentQueryKey>();
            var limitQuery = _queries.OfType<SphinxFluentQueryLimit>().FirstOrDefault();
            var match = string.Join(" ", matchQueries.Where(_ => _.ToString() != null));
            if (match != string.Empty) match = $" MATCH('{match}')";
            var notNullKeys = keyQueries.Where(_ => _.ToString() != null);
            return selectQuery.ToString() + (match != null || notNullKeys.Any() ? " WHERE " : "") +
                match + (notNullKeys.Any()  ? " AND " : "") +
                string.Join(" AND ", notNullKeys) +
                limitQuery?.ToString() + ";show meta;";
        }
        
        
    }

    #region Классы описывающие запросы WHERE в сфинксе

    public class SphinxFluentQuerySelect : SphinxFluentQuery
    {
        private readonly string _index;
        public SphinxFluentQuerySelect(string index)
        {
            _index = index;
        }

        public override string ToString()
        {
            return $"SELECT ID FROM {_index} ";
        }
    }

    public class SphinxFluentQueryMatch : SphinxFluentQuery
    {
        private string _fieldName;
        private object _value;
        private Func<object, bool> _predicate;

        public SphinxFluentQueryMatch(string fieldName, object value, Func<object, bool> predicate)
        {
            _fieldName = fieldName;
            _value = value;
            _predicate = predicate;
        }

        public override string ToString()
        {
            if (_predicate(_value)) return $"{_fieldName} {_value}";
            else return null;
        }
    }

    public class SphinxFluentQueryKey : SphinxFluentQuery
    {
        private string _fieldName;
        private object _value;
        private string _comparator;
        private Func<object, bool> _predicate;

        public SphinxFluentQueryKey(string fieldName, string comparator, object value, Func<object, bool> predicate)
        {
            _fieldName = fieldName;
            if (value != null)
            {
                _value = (int)value;
            }
                
            _comparator = comparator;
            _predicate = predicate;
        }

        public override string ToString()
        {
            if (_predicate(_value)) return $"{_fieldName} {_comparator} {_value}";
            else return null;
        }
    }

    public class SphinxFluentQueryLimit : SphinxFluentQuery
    {
        private long _offset;
        private int _count;
        
        public SphinxFluentQueryLimit(long offset, int count)
        {
            _offset = offset;
            _count = count;
        }

        public override string ToString()
        {
            return $" LIMIT {_offset}, {_count}";
        }
    }

    #endregion

}
