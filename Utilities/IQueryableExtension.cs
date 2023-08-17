using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KalumManagement.Utilities
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Pagination<T>(this IQueryable<T> queryable, int number)
        {
            return queryable
                .Skip(5 * number)
                .Take(5);
        }
    }
}