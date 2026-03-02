using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookFair.Core.Models
{
    
    /// Base class that implements the Observer pattern via INotifyPropertyChanged.
    public abstract class ObservableModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// Raises the PropertyChanged event to notify observers of property value changes.
        /// <param name="propertyName">The name of the property that changed (optional, auto-captured)</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        /// Sets a property value and raises PropertyChanged if the value actually changed.
        
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="field">Reference to the backing field</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The name of the property (optional, auto-captured)</param>
        /// <returns>True if the value changed, false otherwise</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
