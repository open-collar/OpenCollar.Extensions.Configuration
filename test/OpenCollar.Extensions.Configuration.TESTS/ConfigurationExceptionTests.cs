using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ConfigurationExceptionTests
    {
        [Fact]
        public void TestException1()
        {
            var x = new ConfigurationException();

            Assert.NotNull(x);
            Assert.Equal($"Exception of type '{typeof(OpenCollar.Extensions.Configuration.ConfigurationException).FullName}' was thrown.", x.Message);
            Assert.Null(x.InnerException);
            Assert.Null(x.ConfigurationPath);
        }

        [Fact]
        public void TestException2()
        {
            const string message = "MESSAGE";

            var x = new ConfigurationException(message);

            Assert.NotNull(x);
            Assert.Equal(message, x.Message);
            Assert.Null(x.InnerException);
            Assert.Null(x.ConfigurationPath);
        }

        [Fact]
        public void TestException3()
        {
            const string message = "MESSAGE";

            var ex = new Exception(message + ".INNER");

            var x = new ConfigurationException(message, ex);

            Assert.NotNull(x);
            Assert.Equal(message, x.Message);
            Assert.Equal(ex, x.InnerException);
            Assert.Null(x.ConfigurationPath);
        }

        [Fact]
        public void TestException4()
        {
            const string message = "MESSAGE";

            var ex = new Exception(message + ".INNER");

            const string path = "PATH";

            var x = new ConfigurationException(path, message, ex);

            Assert.NotNull(x);
            Assert.Equal(message, x.Message);
            Assert.Equal(ex, x.InnerException);
            Assert.Equal(path, x.ConfigurationPath);
        }

        [Fact]
        public void TestException5()
        {
            const string message = "MESSAGE";
            const string path = "PATH";

            var x = new ConfigurationException(path, message);

            Assert.NotNull(x);
            Assert.Equal(message, x.Message);
            Assert.Null(x.InnerException);
            Assert.Equal(path, x.ConfigurationPath);
        }

        [Fact]
        public void TestSerialization()
        {
            const string message = "MESSAGE";

            var ex = new Exception(message + ".INNER");

            const string path = "PATH";

            var x = new ConfigurationException(path, message, ex);

            var serializer = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            byte[] buffer;
            using(var stream = new System.IO.MemoryStream())
            {
                serializer.Serialize(stream, x);
                buffer = stream.ToArray();
            }

            ConfigurationException y = null;
            using(var stream = new System.IO.MemoryStream(buffer))
            {
                y = (ConfigurationException)serializer.Deserialize(stream);
            }

            Assert.NotNull(y);
            Assert.Equal(message, y.Message);
            Assert.Equal(ex.Message, y.InnerException.Message);
            Assert.Equal(path, y.ConfigurationPath);
        }
    }
}