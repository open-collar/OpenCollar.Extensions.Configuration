namespace OpenCollar.Extensions.Configuration.TESTS
{
    public static class EqualityHelper
    {
        public static bool Equals(System.Collections.Generic.Dictionary<string, object> a, System.Collections.Generic.Dictionary<string, object> b)
        {
            if(ReferenceEquals(a, b))
            {
                return true;
            }
            if(ReferenceEquals(a, null))
            {
                return false;
            }
            if(ReferenceEquals(b, null))
            {
                return false;
            }
            if(a.Count != b.Count)
            {
                return false;
            }
            foreach(var pair in a)
            {
                if(!b.TryGetValue(pair.Key, out var bValue))
                {
                    return false;
                }
                if(!Equals(pair.Value, bValue))
                {
                    return false;
                }
            }

            return true;
        }
    }
}