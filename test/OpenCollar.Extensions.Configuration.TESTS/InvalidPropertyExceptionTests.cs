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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class InvalidPropertyExceptionTests
    {
        [Fact]
        public void TestException1()
        {
            var x = new InvalidPropertyException();

            Assert.NotNull(x);
            Assert.Equal($"Exception of type '{typeof(InvalidPropertyException).FullName}' was thrown.", x.Message);
            Assert.Null(x.InnerException);
            Assert.Null(x.PropertyName);
        }

        [Fact]
        public void TestException2()
        {
            const string message = "MESSAGE";

            var x = new InvalidPropertyException(message);

            Assert.NotNull(x);
            Assert.Equal(message, x.Message);
            Assert.Null(x.InnerException);
            Assert.Null(x.PropertyName);
        }

        [Fact]
        public void TestException3()
        {
            const string message = "MESSAGE";

            var ex = new Exception(message + ".INNER");

            var x = new InvalidPropertyException(message, ex);

            Assert.NotNull(x);
            Assert.Equal(message, x.Message);
            Assert.Equal(ex, x.InnerException);
            Assert.Null(x.PropertyName);
        }

        [Fact]
        public void TestException4()
        {
            const string message = "MESSAGE";

            var ex = new Exception(message + ".INNER");

            const string propertyName = "PROPERTY_NAME";

            var x = new InvalidPropertyException(propertyName, message, ex);

            Assert.NotNull(x);
            Assert.Equal(message, x.Message);
            Assert.Equal(ex, x.InnerException);
            Assert.Equal(propertyName, x.PropertyName);
        }

        [Fact]
        public void TestException5()
        {
            const string message = "MESSAGE";
            const string propertyName = "PROPERTY_NAME";

            var x = new InvalidPropertyException(propertyName, message);

            Assert.NotNull(x);
            Assert.Equal(message, x.Message);
            Assert.Null(x.InnerException);
            Assert.Equal(propertyName, x.PropertyName);
        }

        [Fact]
        public void TestSerialization()
        {
            const string message = "MESSAGE";

            var ex = new Exception(message + ".INNER");

            const string propertyName = "PROPERTY_NAME";

            var x = new InvalidPropertyException(propertyName, message, ex);

            var serializer = new BinaryFormatter();

            byte[] buffer;
            using(var stream = new MemoryStream())
            {
                serializer.Serialize(stream, x);
                buffer = stream.ToArray();
            }

            InvalidPropertyException y = null;
            using(var stream = new MemoryStream(buffer))
            {
                y = (InvalidPropertyException)serializer.Deserialize(stream);
            }

            Assert.NotNull(y);
            Assert.Equal(message, y.Message);
            Assert.Equal(ex.Message, y.InnerException.Message);
            Assert.Equal(propertyName, y.PropertyName);
        }
    }
}