using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace iRacingBusinessLayer.Models
{
    abstract public class BaseModel : INotifyPropertyChanged, ISerializable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        abstract public void GetObjectData(SerializationInfo info, StreamingContext context);
        
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            // PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
