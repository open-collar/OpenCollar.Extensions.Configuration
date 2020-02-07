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
            Assert.True(x.LastPropertyAssignmentWasChange);

            eventArgs = null;

            x.IntPropertyA = 88;

            Assert.Null(eventArgs);
            Assert.False(x.LastPropertyAssignmentWasChange);
        }

        [Fact]
        public void TestPropertyChangedException()
        {
            var x = new NotifyPropertyChangedMock();

            x.PropertyChanged += (sender, args) =>
            {
                throw new Exception("TEST");
            };

            Assert.Throws<AggregateException>(() => { x.IntPropertyA = 88; });
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