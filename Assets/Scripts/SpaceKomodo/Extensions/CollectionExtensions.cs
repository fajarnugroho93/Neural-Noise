using System.Collections.Generic;

namespace SpaceKomodo.Extensions
{
    public static class CollectionExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new System.Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    
        public static IList<T> Shuffled<T>(this IEnumerable<T> list)
        {
            var copy = new List<T>(list);
            copy.Shuffle();
            return copy;
        }
    }
}