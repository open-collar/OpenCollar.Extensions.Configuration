using System;
using System.Linq;
using System.Reflection;

namespace OpenCollar.Extensions.Configuration
{
    /// <summary>
    ///     Settings that govern the way in which configuration objects are generated.
    /// </summary>
    public sealed class ConfigurationObjectSettings
    {
        /// <summary>
        ///     The name of the <see cref="Newtonsoft.Json" /> assembly.
        /// </summary>
        private const string NewtonsoftJsonAssemblyName = @"Newtonsoft.Json";

        /// <summary>
        ///     The <see cref="Newtonsoft.Json" /> assembly, loaded lazily.
        /// </summary>
        private static readonly Lazy<Assembly?> _newtonSoftJsonAssembly = new Lazy<Assembly?>(() =>
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var newtonSoftJsonAssembly = assemblies.FirstOrDefault(a => a.FullName.StartsWith(NewtonsoftJsonAssemblyName + @",", System.StringComparison.Ordinal));
            return newtonSoftJsonAssembly;
        }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        ///     The type of the <see cref="Newtonsoft.Json.JsonConverterAttribute" /> class, loaded lazily.
        /// </summary>
        private static readonly Lazy<Type?> _newtonSoftJsonConverterAttributeType = new Lazy<Type?>(() =>
        {
            var newtonSoftJsonAssembly = ConfigurationObjectSettings.NewtonSoftJsonAssembly;
            if(ReferenceEquals(newtonSoftJsonAssembly, null))
            {
                return null;
            }
            var attributeType = newtonSoftJsonAssembly.ExportedTypes.FirstOrDefault(t => t.Name == nameof(Newtonsoft.Json.JsonConverterAttribute));
            return attributeType;
        }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        ///     The type of the <see cref="Newtonsoft.Json.JsonPropertyAttribute" /> class, loaded lazily.
        /// </summary>
        private static readonly Lazy<Type?> _newtonSoftJsonPropertyAttributeType = new Lazy<Type?>(() =>
        {
            var newtonSoftJsonAssembly = ConfigurationObjectSettings.NewtonSoftJsonAssembly;
            if(ReferenceEquals(newtonSoftJsonAssembly, null))
            {
                return null;
            }
            var attributeType = newtonSoftJsonAssembly.ExportedTypes.FirstOrDefault(t => t.Name == nameof(Newtonsoft.Json.JsonPropertyAttribute));
            return attributeType;
        }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationObjectSettings" /> class.
        /// </summary>
        public ConfigurationObjectSettings()
        {
            EnableNewtonSoftJsonSupport = ReferenceEquals(_newtonSoftJsonAssembly.Value, null);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether <see cref="Newtonsoft.Json" /> serialization should be supported.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if <see cref="Newtonsoft.Json" /> serialization should be supported; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        ///     This property will default to <see langword="true" /> if the <see cref="Newtonsoft.Json" /> assembly is
        ///     loaded at the time the settings object is created.
        /// </remarks>
        public bool EnableNewtonSoftJsonSupport { get; set; }

        /// <summary>
        ///     Gets the <see cref="Newtonsoft.Json" /> assembly.
        /// </summary>
        /// <value>
        ///     The <see cref="Newtonsoft.Json" /> assembly. Can be <see langword="null" /> if the assembly has been loaded.
        /// </value>
        internal static Assembly? NewtonSoftJsonAssembly { get { return _newtonSoftJsonAssembly.Value; } }

        /// <summary>
        ///     Gets the type of the <see cref="Newtonsoft.Json.JsonConverterAttribute" /> class, loaded lazily.
        /// </summary>
        /// <value>
        ///     The type of the <see cref="Newtonsoft.Json.JsonConverterAttribute" /> class, loaded lazily.
        /// </value>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Newtonsoft.Json" /> assembly not loaded.
        /// </exception>
        internal static Type NewtonSoftJsonConverterAttributeType
        {
            get
            {
                var value = _newtonSoftJsonConverterAttributeType.Value;
                if(ReferenceEquals(value, null))
                {
                    throw new InvalidOperationException($@"'{NewtonsoftJsonAssemblyName}' assembly not loaded.");
                }
                return value;
            }
        }

        /// <summary>
        ///     Gets the type of the <see cref="Newtonsoft.Json.JsonPropertyAttribute" /> class, loaded lazily.
        /// </summary>
        /// <value>
        ///     The type of the <see cref="Newtonsoft.Json.JsonPropertyAttribute" /> class, loaded lazily.
        /// </value>
        /// <exception cref="InvalidOperationException">
        ///     <see cref="Newtonsoft.Json" /> assembly not loaded.
        /// </exception>
        internal static Type NewtonSoftJsonPropertyAttributeType
        {
            get
            {
                var value = _newtonSoftJsonPropertyAttributeType.Value;
                if(ReferenceEquals(value, null))
                {
                    throw new InvalidOperationException($@"'{NewtonsoftJsonAssemblyName}' assembly not loaded.");
                }
                return value;
            }
        }

        /// <summary>
        ///     Configures the runtime environment based upon the settings.
        /// </summary>
        internal void ConfigureEnvironment()
        {
            if(EnableNewtonSoftJsonSupport && !_newtonSoftJsonAssembly.IsValueCreated || ReferenceEquals(_newtonSoftJsonAssembly.Value, null))
            {
                System.Reflection.Assembly.Load(NewtonsoftJsonAssemblyName);
            }
        }
    }
}