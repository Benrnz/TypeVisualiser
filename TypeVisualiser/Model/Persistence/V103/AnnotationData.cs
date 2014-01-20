using System;
using System.ComponentModel;
using System.Windows;

namespace TypeVisualiser.Model.Persistence.V103
{
    public class AnnotationData : INotifyPropertyChanged
    {
        private string text;
        private Point topLeft;

        public AnnotationData()
        {
            Id = Guid.NewGuid();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IDiagramContent"/> is shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if shown; otherwise, <c>false</c>.
        /// </value>
        public bool Show { get; set; }

        public string Text
        {
            get { return this.text; }

            set
            {
                this.text = value;
                RaisePropertyChanged("Text");
            }
        }

        public Point TopLeft
        {
            get { return this.topLeft; }

            set
            {
                this.topLeft = value;
                RaisePropertyChanged("TopLeft");
            }
        }

        private void RaisePropertyChanged(string property)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}