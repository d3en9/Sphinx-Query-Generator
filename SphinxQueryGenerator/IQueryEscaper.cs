using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphinxQueryGenerator
{
    public interface IQueryEscaper
    {
        string Escape(string str);
    }
}
