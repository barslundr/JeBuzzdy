using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Jbz.Services;
using Jbz.Views;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using Jbz.Events;
using Jbz.Models;

namespace Jbz.ViewModels
{
    public class MainViewModel : NotificationObject
    {
        private readonly Window _owner;
        private readonly IEventAggregator _eventAggregator;
        private readonly RoundService _roundService;
        private readonly StandingsService _standingsService;
        private readonly InputService _inputService;

        public MainViewModel(Window owner, IEventAggregator eventAggregator, RoundService roundService, StandingsService standingsService, InputService inputService)
        {
            _owner = owner;
            _eventAggregator = eventAggregator;
            _roundService = roundService;
            _standingsService = standingsService;
            _inputService = inputService;

            _roundService.PropertyChanged += (s, e) =>
                                                 {
                                                     if (e.PropertyName.Equals("CurrentRound"))
                                                     {
                                                         RaisePropertyChanged(() => Category1);
                                                         RaisePropertyChanged(() => Category2);
                                                         RaisePropertyChanged(() => Category3);
                                                         RaisePropertyChanged(() => Category4);
                                                         RaisePropertyChanged(() => Category5);
                                                     }
                                                 };

            _standingsService.PropertyChanged += (s, e) =>
                                                     {
                                                         if(e.PropertyName.Equals("IsScoresShown"))
                                                         {
                                                             RaisePropertyChanged(() => IsScoresShown);
                                                         }
                                                     };
        }

        private void ChangeRound()
        {
            throw new NotImplementedException();
        }

        public void CategoryClicked(string category)
        {
            string cat = category.Substring(1, 1);
            string points = category.Substring(3, 1);

            if(!CanSelect(cat, points))
                return;

            var key = int.Parse(cat)-1;
            var value = int.Parse(points)-1;

            _eventAggregator.GetEvent<CategoryClickedEvent>().Publish(new KeyValuePair<int, int>(key, value));

            var q = _roundService.CurrentRound.Questions[new KeyValuePair<int, int>(key, value)];
            var qtype = q.GetType();

            if(qtype == typeof(TextQuestion))
            {
                var viewModel = new TextQuestionWindowViewModel(_eventAggregator, _standingsService, _inputService);
                var view = new TextQuestionWindowView(viewModel)
                               {
                                   Owner = _owner,
                                   Left = _owner.Left - 1,
                                   Width = _owner.ActualWidth,
                                   Top = _owner.Top,
                                   Height = _owner.ActualHeight
                               };
                view.Show();
            }
            if (qtype == typeof(PictureQuestion))
            {
                var viewModel = new PictureQuestionWindowViewModel(_eventAggregator, _standingsService, _inputService)
                                    {
                                        Path = ((PictureQuestion) q).Path,
                                        IsActive = false
                                    };
                var view = new PictureQuestionWindowView(viewModel)
                {
                    Owner = _owner,
                    Left = _owner.Left - 1,
                    Width = _owner.ActualWidth,
                    Top = _owner.Top,
                    Height = _owner.ActualHeight
                };
                view.Show();
            }
            if (qtype == typeof(SoundQuestion))
            {
                var viewModel = new SoundQuestionWindowViewModel(_eventAggregator, _standingsService, _inputService)
                {
                    Path = ((SoundQuestion)q).Path,
                    IsActive = false
                };
                var view = new SoundQuestionWindowView(viewModel)
                {
                    Owner = _owner,
                    Left = _owner.Left - 1,
                    Width = _owner.ActualWidth,
                    Top = _owner.Top,
                    Height = _owner.ActualHeight
                };
                view.Show();
            }
            if (qtype == typeof(VideoQuestion))
            {
                var viewModel = new VideoQuestionWindowViewModel(_eventAggregator, _standingsService, _inputService)
                {
                    Path = ((VideoQuestion)q).Path,
                    IsActive = false
                };
                var view = new VideoQuestionWindowView(viewModel)
                {
                    Owner = _owner,
                    Left = _owner.Left - 1,
                    Width = _owner.ActualWidth,
                    Top = _owner.Top,
                    Height = _owner.ActualHeight
                };
                view.Show();
            }
        }

        private bool CanSelect(string cat, string p)
        {
            switch (cat)
            {
                case "1":
                    return IsPointActive(Category1, p);
                case "2":
                    return IsPointActive(Category2, p);
                case "3":
                    return IsPointActive(Category3, p);
                case "4":
                    return IsPointActive(Category4, p);
                case "5":
                    return IsPointActive(Category5, p);
            }
            return false;
        }

        private bool IsPointActive(CategoryModel category, string p)
        {
            bool res;
            switch (p)
            {
                case "1":
                    res = category.Points1Active;
                    category.Points1Active = false;
                    return res;
                case "2":
                    res = category.Points2Active;
                    category.Points2Active = false;
                    return res;
                case "3":
                    res = category.Points3Active;
                    category.Points3Active = false;
                    return res;
                case "4":
                    res = category.Points4Active;
                    category.Points4Active = false;
                    return res;
                case "5":
                    res = category.Points5Active;
                    category.Points5Active = false;
                    return res;
            }
            return false;
        }

        public CategoryModel Category1
        {
            get { return _roundService.CurrentRound.Categories[0]; }
        }

        public CategoryModel Category2
        {
            get { return _roundService.CurrentRound.Categories[1]; }
        }

        public CategoryModel Category3
        {
            get { return _roundService.CurrentRound.Categories[2]; }
        }

        public CategoryModel Category4
        {
            get { return _roundService.CurrentRound.Categories[3]; }
        }

        public CategoryModel Category5
        {
            get { return _roundService.CurrentRound.Categories[4]; }
        }

        public Player Player1 { get { return _standingsService.Players[0]; } }
        public Player Player2 { get { return _standingsService.Players[1]; } }
        public Player Player3 { get { return _standingsService.Players[2]; } }
        public Player Player4 { get { return _standingsService.Players[3]; } }
        public Player Player5 { get { return _standingsService.Players[4]; } }

        public bool IsScoresShown { get { return _standingsService.IsScoresShown; } }
    }
}
