using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jbz.Models;
using Microsoft.Practices.Prism.ViewModel;

namespace Jbz.Services
{
    public class StandingsService : NotificationObject
    {
        public StandingsService(int players)
        {
            Players = new List<Player>();
            for (int i = 0; i < players; i++)
            {
                Players.Add(new Player());
            }
        }

        private List<Player> _players;

        public List<Player> Players
        {
            get { return _players; }
            set { _players = value; }
        }

        private
            int
            _timeleft;

        public int TimeLeft
        {
            get { return _timeleft; }
            set
            {
                _timeleft = value;
                RaisePropertyChanged(() => TimeLeft);
            }
        }

        private bool _isScoresShown;

        public bool IsScoresShown
        {
            get { return _isScoresShown; }
            set
            {
                _isScoresShown = value;
                RaisePropertyChanged(() => IsScoresShown);
            }
        }

        public void ToggleShowScores()
        {
            IsScoresShown = !IsScoresShown;
        }
    }
}
    