using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class PathHelperTests
    {
        [Fact]
        public void TestGetPath()
        {
            Assert.Equal(string.Empty, PathHelper.GetPath());
            Assert.Equal(string.Empty, PathHelper.GetPath(string.Empty));
            Assert.Equal(string.Empty, PathHelper.GetPath(null));
            Assert.Equal(string.Empty, PathHelper.GetPath(new string[0]));
            Assert.Equal("A:B", PathHelper.GetPath("A", "B"));
            Assert.Equal("A:B", PathHelper.GetPath("A", string.Empty, "B"));
            Assert.Equal("A:B", PathHelper.GetPath("A", null, "B"));
        }
    }
}