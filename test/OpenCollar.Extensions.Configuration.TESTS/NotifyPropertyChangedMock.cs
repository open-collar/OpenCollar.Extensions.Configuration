using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCollar.Extensions.Configuration.TESTS
{
    public sealed class NotifyPropertyChangedMock : NotifyPropertyChanged
    {
        private int _propertyA;

        public int IntPropertyA
        {
            get
            {
                return _propertyA;
            }
            set
            {
                LastPropertyAssignmentWasChange = OnPropertyChanged(nameof(IntPropertyA), ref _propertyA, value);
            }
        }

        public bool LastPropertyAssignmentWasChange
        {
            get; set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether property changed events are fired.
        /// </summary>
        /// <value> <see langword="true" /> if property changed events are suspended; otherwise, <see langword="false" />. </value>
        public new bool SuspendPropertyChangedEvents
        {
            get
            {
                return base.SuspendPropertyChangedEvents;
            }
            set
            {
                base.SuspendPropertyChangedEvents = value;
            }
        }
    }
}