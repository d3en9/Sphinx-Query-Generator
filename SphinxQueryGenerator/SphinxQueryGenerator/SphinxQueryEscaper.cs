using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SphinxQueryGenerator
{
    /// <summary>
    /// Экранирование специальных символов полученных из строки поиска для подстановки в запрос сфинкса
    /// </summary>
    public class SphinxQueryEscaper : IQueryEscaper
    {
        public string Escape(string str)
        {
            return Regex.Replace(str, @"([\'\(\)|\-!@~\""&/\\\^\$\=])", "\\$1");
        }
    }
}
