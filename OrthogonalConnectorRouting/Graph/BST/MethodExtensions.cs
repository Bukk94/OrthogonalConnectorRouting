using System.Collections.Generic;

namespace OrthogonalConnectorRouting.Graph
{
    public static class MethodExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }
    }
}
