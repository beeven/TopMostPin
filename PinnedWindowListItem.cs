using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace TopMostPin
{
    public class PinnedWindowListItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void Changed(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _pinned;
        private IntPtr _handle;
        private string _title;

        public bool Pinned
        {
            get { return _pinned; }
            set
            {
                if (value != _pinned)
                {
                    _pinned = value;
                    Changed(nameof(Pinned));
                }
            }
        }


        public IntPtr Handle
        {
            get => _handle;

            set
            {
                if (value != _handle)
                {
                    _handle = value;
                    Changed(nameof(Handle));
                }

            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if(value != _title)
                {
                    _title = value;
                    Changed(nameof(Title));
                }
                
            }
        }
    }
}
