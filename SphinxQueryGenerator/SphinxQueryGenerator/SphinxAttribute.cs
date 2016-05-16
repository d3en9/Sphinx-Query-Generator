using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphinxQueryGenerator
{
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SphinxAttribute : Attribute
    {
        public string FieldName;

        public SphinxAttribute()
        {

        }

        public SphinxAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }

    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SphinxLimitFromAttribute : SphinxAttribute
    {
        public SphinxLimitFromAttribute()
        {

        }
    }

    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SphinxLimitTakeAttribute : SphinxAttribute
    {
        public SphinxLimitTakeAttribute()
        {

        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class SphinxMatchAttribute : SphinxAttribute
    {
        protected string _groupName;
        public SphinxMatchAttribute(string fieldName, string groupName = null) : base(fieldName)
        {
            _groupName = groupName;
        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class SphinxInAttribute : SphinxAttribute
    {
        public Type ItemType { get; private set; }

        public SphinxInAttribute(string fieldName, Type itemType) : base(fieldName)
        {
            ItemType = itemType;
        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class SphinxInIntAttribute : SphinxInAttribute
    {
        public SphinxInIntAttribute(string fieldName) : base(fieldName, typeof(int))
        {

        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class SphinxInLongAttribute : SphinxInAttribute
    {
        public SphinxInLongAttribute(string fieldName) : base(fieldName, typeof(long))
        {

        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class SphinxIdAttribute : SphinxAttribute
    {
        public int[] NullValues;
        public ComparisonType Comparison;
        public SphinxIdAttribute(string fieldName, ComparisonType comparison = ComparisonType.Equal, int[] nullValues = null) : base(fieldName)
        {
            NullValues = nullValues;
            Comparison = comparison;
        }
    }

    [System.AttributeUsage(AttributeTargets.Property)]
    public class SphinxConstantAttribute : SphinxAttribute
    {
        public ComparisonType Comparison;
        public object Value;
        public SphinxConstantAttribute(string fieldName, object value, ComparisonType comparison = ComparisonType.Equal) : base(fieldName)
        {
            Comparison = comparison;
            Value = value;
        }
    }

    public enum ComparisonType
    {
        [Description("=")]
        Equal = 0,
        [Description(">=")]
        Ge,
        [Description("<=")]
        Le,
        [Description("<>")]
        NotEq
    }
}
