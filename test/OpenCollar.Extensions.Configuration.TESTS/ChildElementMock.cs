using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class ChildElementMock : IChildElement
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsDirty
        {
            get;
            set;
        }

        public bool IsReadOnly
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public PropertyDef PropertyDef
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }

        public void Delete() => throw new NotImplementedException();

        public void Dispose() => throw new NotImplementedException();

        public string GetPath() => throw new NotImplementedException();

        public void Load() => throw new NotImplementedException();

        public void Save() => throw new NotImplementedException();
    }
}