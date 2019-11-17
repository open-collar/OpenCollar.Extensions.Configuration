using System;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    /// An enumeration of the ways in which the string supplied to the <see cref="PathAttribute">Path</see> attribute
    /// can be used to create a full path.
    /// </summary>
    public enum PathIs
    {
        /// <summary>The usage of the path is unknown or undefined.  Use of this value will usually result in an
        /// error; it is provided to as sentinel to detect accidental usages.</summary>
        Unknown = 0,

        /// <summary>The path is treated as a root and any previous context is ignored.</summary>
        Root,

        /// <summary>The path is treated as a suffix to be applied (as part of semi-colon delited list) to the
        /// existing path context.</summary>
        Suffix
    }

    /// <summary>
    /// Defines an attribute used to indicate the path to the configuration value(s) underlying a class or individual property.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class PathAttribute : System.Attribute
    {
        /// <summary>
        /// Gets the usage of the path or fragment of a path specified.
        /// </summary>
        /// <value>
        /// The usage of the path or fragment of a path specified.
        /// </value>
        internal PathIs Usage
        {
            get;
        }

        /// <summary>
        /// Gets the full path or fragment of a path specified.
        /// </summary>
        /// <value>
        /// The full path or fragment of a path specified.
        /// </value>
        internal string Path
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PathAttribute" /> class.
        /// </summary>
        /// <param name="usage">The usage.</param>
        /// <param name="path">The path.</param>
        /// <exception cref="ArgumentException"><paramref name="path"/> must contain a valid path or fragment of a path.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="usage"/> does not contain a valid value.</exception>
        public PathAttribute(PathIs usage, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException($"'{nameof(path)}' must contain a valid path or fragment of a path.", nameof(path));
            }

            switch (usage)
            {
                case PathIs.Root:
                case PathIs.Suffix:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(usage), usage, "'usage' does not contain a valid value.");
            }

            Usage = usage;
            Path = path;
        }
    }
}
