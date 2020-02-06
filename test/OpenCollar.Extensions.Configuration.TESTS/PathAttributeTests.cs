using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class PathAttributeTests
    {
        [Fact]
        public void TestConstructor()
        {
            const string path = @"PATH";
            var x = new PathAttribute(PathIs.Root, path);

            Assert.NotNull(x);
            Assert.Equal(path, x.Path);
            Assert.Equal(PathIs.Root, x.Usage);

            Assert.Throws<ArgumentException>(() => { new PathAttribute(PathIs.Root, null); });
            Assert.Throws<ArgumentException>(() => { new PathAttribute(PathIs.Root, string.Empty); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new PathAttribute(PathIs.Unknown, path); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new PathAttribute((PathIs)666, path); });
        }
    }
}