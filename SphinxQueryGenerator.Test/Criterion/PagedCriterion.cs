using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SphinxQueryGenerator.Test
{
    /// <summary>
    /// Базовый класс для моделей постраничного вывода
    /// </summary>
    public class PagedCriterion : ICriterion
    {
        public int PageNum { get; set; }
        public int PageSize { get; set; }

        public long Offset
        {
            get
            {
                return PageSize * (PageNum - 1);
            }
        }
    }
}
