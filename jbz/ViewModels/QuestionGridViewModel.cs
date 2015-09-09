using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Jbz.Events;
using Jbz.Models;
using Jbz.Services;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace Jbz.ViewModels
{
    public class QuestionGridViewModel : NotificationObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly RoundService _roundService;
        private readonly StandingsService _standingsService;
        private string _points;
        private ICommand _addCommand;
        private ICommand _substractCommand;

        private ICommand _startCommand;
        private ICommand _stopCommand;
        private ICommand _bonusCommand;
        private ICommand _wrongCommand;
        private ICommand _correctCommand;
        private ICommand _closeCommand;
        private ICommand _bonusAddCommand;

        private ShowEvent _showEvent;
        private ActivateEvent _activateEvent;
        private StopEvent _stopEvent;
        private BonusEvent _bonusEvent;
        private CloseEvent _closeEvent;
        private NewSelectorEvent _newSelectorEvent;

        public ICommand CloseCommand
        {
            get { return _closeCommand; }
            set { _closeCommand = value; }
        }

        public ICommand CorrectCommand
        {
            get { return _correctCommand; }
            set { _correctCommand = value; }
        }

        public ICommand WrongCommand
        {
            get { return _wrongCommand; }
            set { _wrongCommand = value; }
        }

        public ICommand BonusCommand
        {
            get { return _bonusCommand; }
            set { _bonusCommand = value; }
        }

        public ICommand StopCommand
        {
            get { return _stopCommand; }
            set { _stopCommand = value; }
        }

        public ICommand StartCommand
        {
            get { return _startCommand; }
            set { _startCommand = value; }
        }
        
        public ICommand BonusAddCommand
        {
            get { return _bonusAddCommand; }
            set { _bonusAddCommand = value; }
        }

        private KeyValuePair<int, int> _index;

        public KeyValuePair<int, int> Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public QuestionGridViewModel(IEventAggregator eventAggregator, RoundService roundService, StandingsService standingsService)
        {
            _eventAggregator = eventAggregator;
            _roundService = roundService;
            _standingsService = standingsService;

            AddCommand = new DelegateCommand<object>(AddPoints);
            SubstractCommand = new DelegateCommand<object>(Substract);

            StartCommand = new DelegateCommand(Start);
            StopCommand = new DelegateCommand(Stop);
            BonusCommand = new DelegateCommand(Bonus);
            WrongCommand = new DelegateCommand(Wrong);
            CorrectCommand = new DelegateCommand(Correct);
            CloseCommand = new DelegateCommand(Close);
            BonusAddCommand = new DelegateCommand<object>(AddBonusPoints);

            _showEvent = _eventAggregator.GetEvent<ShowEvent>();
            _activateEvent = _eventAggregator.GetEvent<ActivateEvent>();
            _stopEvent = _eventAggregator.GetEvent<StopEvent>();
            _bonusEvent = _eventAggregator.GetEvent<BonusEvent>();
            _closeEvent = _eventAggregator.GetEvent<CloseEvent>();

            _newSelectorEvent = _eventAggregator.GetEvent<NewSelectorEvent>();
        }

        public ICommand AddCommand
        {
            get { return _addCommand; }
            set
            {
                _addCommand = value;
                RaisePropertyChanged(() => AddCommand);
            }
        }

        public ICommand SubstractCommand
        {
            get { return _substractCommand; }
            set
            {
                _substractCommand = value;
                RaisePropertyChanged(() => SubstractCommand);
            }
        }

        public string Points
        {
            get { return _points; }
            set
            {
                _points = value;
                RaisePropertyChanged(() => Points);
            }
        }


        private void Substract(object obj)
        {
            int points = 0;
            Int32.TryParse(Points, out points);

            int p = -1;
            if (Int32.TryParse((string)obj, out p))
                _standingsService.Players[p-1].Points -= points;
        }

        private void AddPoints(object obj)
        {
            int points = 0;
            Int32.TryParse(Points, out points);

            int p = -1;
            if (Int32.TryParse((string)obj, out p))
            {
                _newSelectorEvent.Publish(p);
                _standingsService.Players[p - 1].Points += points;
            }
        }


        public void Start()
        {
            _showEvent.Publish(true);
            _activateEvent.Publish(true);
        }

        public void Stop()
        {
            _showEvent.Publish(false);
            _activateEvent.Publish(false);
        }

        public void Bonus()
        {
            _showEvent.Publish(true);
        }

        public void Wrong()
        {
            _showEvent.Publish(true);
            _activateEvent.Publish(true);
        }

        public void Correct()
        {
            Stop();
        }

        public void Close()
        {
            Stop();
            _closeEvent.Publish(Index);
            Save();
        }

        private void Save()
        {
            using (var writer = new StreamWriter(@"backup.txt", false))
            {
                foreach (var pl in _standingsService.Players)
                {
                    writer.WriteLine(string.Format("{0}:\t{1}", pl.Name, pl.Points));
                }

                var str1 = string.Empty;
                var str2 = string.Empty;
                var str3 = string.Empty;
                var str4 = string.Empty;
                var str5 = string.Empty;
                var str6 = string.Empty;
                foreach (CategoryModel t in _roundService.CurrentRound.Categories)
                {
                    str1 += t.Category + "\t";
                    str2 += t.Points1Active + "\t";
                    str3 += t.Points2Active + "\t";
                    str4 += t.Points3Active + "\t";
                    str5 += t.Points4Active + "\t";
                    str6 += t.Points5Active + "\t";
                }
                writer.WriteLine();
                writer.WriteLine(str1);
                writer.WriteLine(str2);
                writer.WriteLine(str3);
                writer.WriteLine(str4);
                writer.WriteLine(str5);
                writer.WriteLine(str6);
            }
        }

        private void AddBonusPoints(object i)
        {
            try
            {
                Points = (Int32.Parse(Points) + Int32.Parse((string)i)).ToString();
            }
            catch (Exception)
            {
                Points = "0";
            }
        }
    }
}
