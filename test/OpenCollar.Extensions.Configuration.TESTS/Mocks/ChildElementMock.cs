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

#pragma warning disable CS0067 // Interface requirement

using System;
using System.ComponentModel;

using JetBrains.Annotations;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    [UsedImplicitly]
    internal sealed class ChildElementMock : IChildElement
    {
        public bool IsReadOnly
        {
            get; set;
        }

        public IPropertyDef PropertyDef
        {
            get; set;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDirty
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public int Value
        {
            get; set;
        }

        public void Delete() => throw new NotImplementedException();

        public void Dispose() => throw new NotImplementedException();

        public void Load() => throw new NotImplementedException();

        public void Save() => throw new NotImplementedException();

        public string CalculatePath() => throw new NotImplementedException();
    }
}