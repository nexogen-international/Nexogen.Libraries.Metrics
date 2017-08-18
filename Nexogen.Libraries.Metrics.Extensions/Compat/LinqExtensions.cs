#if NET452
#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    internal static class LinqExtensions
    {
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> tail, T head)
        {
            return new T[] { head }.Concat(tail);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> head, T tail)
        {
            return head.Concat(new T[] { tail });
        }
    }
}
#endif
