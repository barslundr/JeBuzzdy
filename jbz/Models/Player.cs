using Microsoft.Practices.Prism.ViewModel;

namespace Jbz.Models
{
    public class Player : NotificationObject
    {
        private string _name;
        private int _points;
        private bool _isActive;
        private bool _isAvailable = true;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged(() => State);
            }
        }

        public bool IsAvailable
        {
            get { return _isAvailable; }
            set
            {
                _isAvailable = value;
                RaisePropertyChanged(() => State);
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public int Points
        {
            get { return _points; }
            set
            {
                _points = value;
                RaisePropertyChanged(() => Points);
            }
        }
        
        public int State
        {
            get
            {
                if (!IsAvailable && !IsActive)
                    return 0;
                else if (IsAvailable && !IsActive)
                    return 1;
                else if (!IsAvailable && IsActive)
                    return 3;
                else if (IsAvailable && IsActive)
                    return 3;
                else 
                    return 2;
            }
        }
    }
}