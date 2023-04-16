using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace iRacingSchedule.Base
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
