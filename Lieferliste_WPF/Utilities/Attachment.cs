using System.ComponentModel;

namespace Lieferliste_WPF.Utilities
{
    internal class Attachment(int id, bool isLink) : INotifyPropertyChanged
    {
        private string _name = string.Empty;

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
        public readonly int Ident = id;
        public readonly bool IsLink = isLink;
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

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
