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

namespace OpenCollar.Extensions.Configuration.Converter
{
    /// <summary>
    ///     A converter allowing <see cref="IConfigurationDictionary{TElement}" /> derived objects to be serialized and
    ///     deserialized as JSON objects.
    /// </summary>
    /// <typeparam name="TElement">
    ///     The type of the element.
    /// </typeparam>
    /// <seealso cref="JsonConverter{T}" />
    public class ConfigurationDictionaryConverter<TElement> : JsonConverter<IConfigurationDictionary<TElement>>
    {
        /// <summary>
        ///     The type of the elements in the dictionary.
        /// </summary>
        private readonly Type _elementType;

        /// <summary>
        ///     The type of the key.
        /// </summary>
        private readonly Type _keyType;

        /// <summary>
        ///     The JSON converter for the type of the element specified.
        /// </summary>
        private readonly JsonConverter<TElement> _valueConverter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationDictionaryConverter{TElement}" /> class.
        /// </summary>
        /// <param name="options">
        ///     The options controlling the serialization/deserialization.
        /// </param>
        public ConfigurationDictionaryConverter(JsonSerializerOptions options)
        {
            // For performance, use the existing converter if available.
            _valueConverter = (JsonConverter<TElement>)options
                .GetConverter(typeof(TElement));

            // Cache the key and value types.
            _keyType = typeof(string);
            _elementType = typeof(TElement);
        }

        /// <summary>
        ///     Reads and converts the JSON to type <typeparamref name="T" />.
        /// </summary>
        /// <param name="reader">
        ///     The reader from which to take JSON elements.
        /// </param>
        /// <param name="typeToConvert">
        ///     The type to convert.
        /// </param>
        /// <param name="options">
        ///     An object that specifies serialization options to use.
        /// </param>
        /// <returns>
        ///     The converted value.
        /// </returns>
        /// <exception cref="JsonException">
        ///     Unable to convert \"{propertyName}\" to Enum \"{_keyType}\". or
        /// </exception>
        public override IConfigurationDictionary<TElement> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            //if(reader.TokenType != JsonTokenType.StartObject)
            //{
            //    throw new JsonException();
            //}

            //IConfigurationDictionary<TElement> dictionary = new ConfigurationDictionary<TElement>();

            //while(reader.Read())
            //{
            //    if(reader.TokenType == JsonTokenType.EndObject)
            //    {
            //        return dictionary;
            //    }

            // // Get the key. if(reader.TokenType != JsonTokenType.PropertyName) { throw new JsonException(); }

            // string propertyName = reader.GetString();

            // // For performance, parse with ignoreCase:false first. if(!Enum.TryParse(propertyName, ignoreCase: false,
            // out TKey key) && !Enum.TryParse(propertyName, ignoreCase: true, out key)) { throw new JsonException(
            // $"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\"."); }

            // // Get the value. TElement v; if(_valueConverter != null) { reader.Read(); v = _valueConverter.Read(ref
            // reader, _elementType, options); } else { v = JsonSerializer.Deserialize<TElement>(ref reader, options); }

            //    // Add to dictionary.
            //    dictionary.Add(key, v);
            //}

            throw new JsonException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            IConfigurationDictionary<TElement> dictionary,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach(var kvp in dictionary)
            {
                writer.WritePropertyName(kvp.Key.ToString());

                if(_valueConverter != null)
                {
                    _valueConverter.Write(writer, kvp.Value, options);
                }
                else
                {
                    JsonSerializer.Serialize(writer, kvp.Value, options);
                }
            }

            writer.WriteEndObject();
        }
    }

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