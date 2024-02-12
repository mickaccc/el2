using System.ComponentModel;

namespace Lieferliste_WPF.Utilities
{
    internal class Attachment : INotifyPropertyChanged
    {
        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        private string? description;

        public string? Description
        {
            get
            {
                return description;
            }
            set
            {
                if (value != description)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public readonly int Ident;
        public readonly bool IsLink;
        private object? content;

        public object? Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value != content)
                {
                    content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        public enum KnownTypes
        {
            Unknow,
            Picture,
            Pdf,
            Mail
        }
        public Attachment(int id, bool isLink) { Ident = id; IsLink = isLink; }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
