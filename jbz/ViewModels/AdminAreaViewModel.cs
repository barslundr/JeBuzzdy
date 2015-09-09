using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Jbz.Models;
using Jbz.Services;
using Jbz.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Jbz.Events;

namespace Jbz.ViewModels
{
    public class AdminAreaViewModel : NotificationObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly RoundService _roundService;
        private readonly StandingsService _standingsService;
        private readonly InputService _inputService;
        private string _question;
        private string _answer;
        private bool _isBonus;
        private Player _isSelecting;

        public int TimeLeft
        {
            get { return _standingsService.TimeLeft; }
        }

        public Player CurrentActive
        {
            get
            {
                return _inputService.ActiveController>0 ? _standingsService.Players[_inputService.ActiveController-1] : null;
            }
        }

        public Player IsSelecting
        {
            get { return _isSelecting; }
            set
            {
                _isSelecting = value;
                RaisePropertyChanged(() => IsSelecting);
            }
        }

        public string Answer
        {
            get { return _answer; }
            set
            {
                _answer = value;
                RaisePropertyChanged(() => Answer);
            }
        }

        private QuestionGridViewModel _questionGridViewModel;
        private int _round;
        
        public QuestionGridViewModel QuestionGridViewModel
        {
            get { return _questionGridViewModel; }
            set
            {
                _questionGridViewModel = value;
                RaisePropertyChanged(() => QuestionGridViewModel);
            }
        }

        public string Question
        {
            get { return _question; }
            set
            {
                _question = value;
                RaisePropertyChanged(() => Question);

                Reset();
            }
        }

        private void Reset()
        {
            InputText = string.Empty;
        }

        public bool IsBonus
        {
            get { return _isBonus; }
            set
            {
                _isBonus = value;
                RaisePropertyChanged(() => IsBonus);
            }
        }


        public AdminAreaViewModel(IEventAggregator eventAggregator, QuestionGridViewModel questionGridViewModel, 
            RoundService roundService, StandingsService standingsService, InputService inputService)
        {
            _eventAggregator = eventAggregator;
            _roundService = roundService;
            _standingsService = standingsService;
            _inputService = inputService;
            QuestionGridViewModel = questionGridViewModel;
            _round = 1;
            ChangeRoundCommand = new DelegateCommand<object>(ChangedRound);
            ShowScoresCommand = new DelegateCommand(() => _standingsService.ToggleShowScores());
            IsSelecting = _standingsService.Players[new Random().Next(0, 4)];
            _eventAggregator.GetEvent<CategoryClickedEvent>().Subscribe(DisplayQuestion);
            _eventAggregator.GetEvent<ActivePlayerChangedEvent>().Subscribe(ActivePlayerChangedEventHandler);

            _eventAggregator.GetEvent<NewSelectorEvent>().Subscribe((i) =>
                {
                    IsSelecting = _standingsService.Players[i - 1];
                });
            
            _inputService.PropertyChanged += 
                (s, e) =>
                    {
                        if(e.PropertyName.Equals("ActiveController"))
                            RaisePropertyChanged(() => CurrentActive);
                    };

            _standingsService.PropertyChanged +=
                (s, e) =>
                    {
                        if (e.PropertyName.Equals("TimeLeft"))
                            RaisePropertyChanged(() => TimeLeft);
                    };
        }
        
        private void ActivePlayerChangedEventHandler(object obj)
        {
            InputText += "\n" + (string) obj;
        }

        private string _inputText;

        public string InputText
        {
            get { return _inputText; }
            set
            {
                _inputText = value;
                RaisePropertyChanged(() => InputText);
            }
        }

        public InputService InputService
        {
            get { return _inputService; }
        }


        private void DisplayQuestion(KeyValuePair<int,int> payload)
        {
            Reset();
            QuestionGridViewModel.Close();
            Question question = _roundService.CurrentRound.Questions[payload];
            var category = _roundService.CurrentRound.Categories[payload.Key];
            Question = question.GetType().Name + ":\n" + question.Quest;
            Answer = question.Answer;
            IsBonus = question.IsBonus;
            QuestionGridViewModel.Points = category.Points[payload.Value];
            QuestionGridViewModel.Index = payload;
        }




        public string Player1
        {
            get { return _standingsService.Players[0].Name; }
            set
            {
                _standingsService.Players[0].Name = value;
                RaisePropertyChanged(() => Player1);
            }
        }
        public string Player2
        {
            get { return _standingsService.Players[1].Name; }
            set
            {
                _standingsService.Players[1].Name = value;
                RaisePropertyChanged(() => Player2);
            }
        }
        public string Player3
        {
            get { return _standingsService.Players[2].Name; }
            set
            {
                _standingsService.Players[2].Name = value;
                RaisePropertyChanged(() => Player3);
            }
        }
        public string Player4
        {
            get { return _standingsService.Players[3].Name; }
            set
            {
                _standingsService.Players[3].Name = value;
                RaisePropertyChanged(() => Player4);
            }
        }
        public string Player5
        {
            get { return _standingsService.Players[4].Name; }
            set
            {
                _standingsService.Players[4].Name = value;
                RaisePropertyChanged(() => Player5);
            }
        }

        public List<Player> Players
        {
            get { return _standingsService.Players; }
        }

        public ICommand ChangeRoundCommand { get; set; }

        public ICommand ShowScoresCommand { get; set; }
        
        private void ChangedRound(object round)
        {
            if(round.Equals("2"))
                _roundService.CurrentRound = _roundService.Round2;
            else
                _roundService.CurrentRound = _roundService.Final;
        }
    }
}
