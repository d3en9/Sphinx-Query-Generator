using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SphinxQueryGenerator
{
    public static class EnumHelper
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            string description = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(description);
            DescriptionAttribute[] attributes =
               (DescriptionAttribute[])
             fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                description = attributes[0].Description;
            }
            return description;
        }
    }
}
