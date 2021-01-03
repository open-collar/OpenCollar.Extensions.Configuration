/*
 * This file is part of OpenCollar.Extensions.Configuration.
 *
 * OpenCollar.Extensions.Configuration is free software: you can redistribute it
 * and/or modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * OpenCollar.Extensions.Configuration is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public
 * License for more details.
 *
 * You should have received a copy of the GNU General Public License along with
 * OpenCollar.Extensions.Configuration.  If not, see <https://www.gnu.org/licenses/>.
 *
 * Copyright © 2019-2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

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
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var newtonSoftJsonAssembly = assemblies.FirstOrDefault(a => a.FullName.StartsWith(NewtonsoftJsonAssemblyName + @",", StringComparison.Ordinal));
            return newtonSoftJsonAssembly;
        }, System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        ///     The type of the <see cref="Newtonsoft.Json.JsonConverterAttribute" /> class, loaded lazily.
        /// </summary>
        private static readonly Lazy<Type?> _newtonSoftJsonConverterAttributeType = new Lazy<Type?>(() =>
        {
            var newtonSoftJsonAssembly = NewtonSoftJsonAssembly;
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
            var newtonSoftJsonAssembly = NewtonSoftJsonAssembly;
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
        public bool EnableNewtonSoftJsonSupport { get; set; } = false;

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
                Assembly.Load(NewtonsoftJsonAssemblyName);
            }
        }
    }
}