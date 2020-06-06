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
 * Copyright © 2020 Jonathan Evans (jevans@open-collar.org.uk).
 */

using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenCollar.Extensions.Configuration.Converters.Text.Json
{
    /// <summary>
    ///     A factory that produces converters allowing objects implementing
    ///     <see cref="IConfigurationDictionary{TElement}" /> to be converted to and from JSON format.
    /// </summary>
    /// <seealso cref="JsonConverterFactory" />
    public class ConfigurationDictionaryConverterFactory : JsonConverterFactory
    {
        /// <summary>
        ///     When overridden in a derived class, determines whether the converter instance can convert the specified
        ///     object type.
        /// </summary>
        /// <param name="typeToConvert">
        ///     The type of the object to check whether it can be converted by this converter instance.
        /// </param>
        /// <returns>
        ///     <see langword="true" /> if the instance can convert the specified object type; otherwise, <see langword="false" />.
        /// </returns>
        public override bool CanConvert(Type typeToConvert)
        {
            if(!typeToConvert.IsGenericType)
            {
                return false;
            }

            if(typeToConvert.GetGenericTypeDefinition() != typeof(IConfigurationDictionary<>))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Creates the converter for the type specified.
        /// </summary>
        /// <param name="type">
        ///     The type to convert.
        /// </param>
        /// <param name="options">
        ///     The options controlling the conversion.
        /// </param>
        /// <returns>
        ///     A JSON converter for the type given.
        /// </returns>
        public override JsonConverter CreateConverter(
            Type type,
            JsonSerializerOptions options)
        {
            var valueType = type.GetGenericArguments()[0];

            var converter = (JsonConverter)Activator.CreateInstance(
                typeof(ConfigurationDictionaryConverter<>).MakeGenericType(new Type[] { valueType }), BindingFlags.Instance | BindingFlags.Public, binder: null, args: new object[] { options }, culture: null);

            return converter;
        }
    }
}