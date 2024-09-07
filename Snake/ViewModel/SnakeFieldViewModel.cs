using System;

namespace Snake.ViewModel
{
    /// <summary>
    /// A használt mező típusa
    /// </summary>
    public class SnakeFieldViewModel : ViewModelBase
    {
        private String _color;

        public Int32 Row { get; set; }
        public Int32 Column { get; set; }
        public String Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
