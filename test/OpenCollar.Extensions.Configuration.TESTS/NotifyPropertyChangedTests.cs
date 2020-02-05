using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class NotifyPropertyChangedTests
    {
        [Fact]
        public void TestPropertyChanged()
        {
            var x = new NotifyPropertyChangedMock();

            System.ComponentModel.PropertyChangedEventArgs eventArgs = null;

            x.PropertyChanged += (sender, args) =>
            {
                eventArgs = args;
            };

            x.IntPropertyA = 88;

            Assert.NotNull(eventArgs);
            Assert.Equal(nameof(NotifyPropertyChangedMock.IntPropertyA), eventArgs.PropertyName);
        }

        [Fact]
        public void TestPropertyChangedNoChange()
        {
            var x = new NotifyPropertyChangedMock();

            x.IntPropertyA = 88;

            System.ComponentModel.PropertyChangedEventArgs eventArgs = null;

            x.PropertyChanged += (sender, args) =>
            {
                eventArgs = args;
            };

            x.IntPropertyA = 88;

            Assert.Null(eventArgs);
        }

        [Fact]
        public void TestPropertyChangedSuspended()
        {
            var x = new NotifyPropertyChangedMock();

            System.ComponentModel.PropertyChangedEventArgs eventArgs = null;

            x.PropertyChanged += (sender, args) =>
            {
                eventArgs = args;
            };

            x.SuspendPropertyChangedEvents = true;

            x.IntPropertyA = 99;

            x.SuspendPropertyChangedEvents = false;

            Assert.Null(eventArgs);
        }
    }
}