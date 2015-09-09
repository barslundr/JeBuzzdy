using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;

namespace Jbz.Models
{
    public class CategoryModel : NotificationObject
    {
        private string _category = string.Empty;
        private string _points1 = "";
        private string _points2 = "";
        private string _points3 = "";
        private string _points4 = "";
        private string _points5 = "";

        public List<string> Points
        {
            get { return new List<string>{Points1, Points2, Points3, Points4, Points5}; }
        }

        private bool _points1Active = false;
        private bool _points2Active = false;
        private bool _points3Active = false;
        private bool _points4Active = false;
        private bool _points5Active = false;

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public string Points1
        {
            get { return _points1; }
            set { _points1 = value; }
        }

        public bool Points1Active
        {
            get { return _points1Active; }
            set
            {
                _points1Active = value;
                RaisePropertyChanged(() => Points1Active);
            }
        }

        public string Points2
        {
            get { return _points2; }
            set { _points2 = value; }
        }

        public bool Points2Active
        {
            get { return _points2Active; }
            set
            {
                _points2Active = value;
                RaisePropertyChanged(() => Points2Active);
            }
        }

        public string Points3
        {
            get { return _points3; }
            set { _points3 = value; }
        }

        public bool Points3Active
        {
            get { return _points3Active; }
            set
            {
                _points3Active = value;
                RaisePropertyChanged(() => Points3Active);
            }
        }

        public string Points4
        {
            get { return _points4; }
            set { _points4 = value; }
        }

        public bool Points4Active
        {
            get { return _points4Active; }
            set
            {
                _points4Active = value;
                RaisePropertyChanged(() => Points4Active);
            }
        }

        public string Points5
        {
            get { return _points5; }
            set { _points5 = value; }
        }

        public bool Points5Active
        {
            get { return _points5Active; }
            set
            {
                _points5Active = value;
                RaisePropertyChanged(() => Points5Active);
            }
        }
    }
}
