﻿namespace OpenCollar.Extensions.Configuration
{
    /// <summary> Support for comparison. </summary>
    internal static class UniversalComparer
    {
        /// <summary> Compares two objects, using custom logic for configuration objects. </summary>
        /// <param name="a"> The first object to compare. </param>
        /// <param name="b"> The second object to compare. </param>
        /// <returns> <see langword="true"/> if the objects are equivalent; otherwise, <see langword="false"/>. </returns>
        public new static bool Equals(object? a, object? b)
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
#pragma warning disable CS8604 // Possible null reference argument.
                return ConfigurationObjectComparer.Instance.Equals(coA, coB);
#pragma warning restore CS8604 // Possible null reference argument.
            }

            return a.Equals(b);
        }
    }
}