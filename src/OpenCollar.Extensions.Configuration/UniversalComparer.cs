namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Support for comparison.
    /// </summary>
    public static class UniversalComparer
    {
        /// <summary>
        ///     Compares two objects, using custom logic for configuration objects.
        /// </summary>
        /// <param name="a"> The first object to compare. </param>
        /// <param name="b"> The second object to compare. </param>
        /// <returns> <see langword="true" /> if the objects are equivalent; otherwise, <see langword="false" />. </returns>
        public static new bool Equals(object? a, object? b)
        {
            if(ReferenceEquals(a, b))
            {
                return true;
            }

            if(ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            var coA = a as IConfigurationObject;
            var coB = b as IConfigurationObject;
            if(!ReferenceEquals(coA, null) && !ReferenceEquals(coA, null))
            {
                return ConfigurationObjectComparer.Instance.Equals(coA, coB);
            }

            return a.Equals(b);
        }
    }
}