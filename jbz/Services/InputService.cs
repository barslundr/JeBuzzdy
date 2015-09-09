using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Input;
using Jbz.Events;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace Jbz.Services
{
    public class InputService : NotificationObject
    {
        private readonly EventAggregator _eventAggregator;
        private readonly StandingsService _standingsService;
        ActivePlayerChangedEvent _activePlayerChangedEvent;
        private ShowEvent _showEvent;

        public InputService(EventAggregator eventAggregator, StandingsService standingsService)
        {
            _eventAggregator = eventAggregator;
            _standingsService = standingsService;

            _showEvent = _eventAggregator.GetEvent<ShowEvent>();
            _activePlayerChangedEvent = _eventAggregator.GetEvent<ActivePlayerChangedEvent>();
            _eventAggregator.GetEvent<ActivateEvent>().Subscribe(Activate);
            _eventAggregator.GetEvent<CloseEvent>().Subscribe(Reset);

            _buzz = new BuzzIOWrite.BuzzIOWrite();
            _buzz.FindTheHid();

            _players = new[] { false, false, false, false, false };
            _pressed = new[] { false, false, false, false, false };
            _used = new[] { false, false, false, false, false };

            _timeleft = TimeAvailable;
            _timer = new Timer(1000) { AutoReset = false };
            _timer.Elapsed += new ElapsedEventHandler(OnTimerEnd);
        }

        private void Reset(KeyValuePair<int, int> obj)
        {
            for(int i=0; i<_players.Length;i++)
            {
                _players[i] = false;
                _used[i] = false;
                _standingsService.Players[i].IsAvailable = true;
                _standingsService.Players[i].IsActive = false;
                _standingsService.TimeLeft = TimeAvailable;
            }
            TurnOffAllLight();
        }

        private void Activate(bool activate)
        {
            _controllersActive = activate;
            if (_controllersActive)
            {
                Stop();
                _timerActive = false;
                TurnOnRemainingLight();
            }
            else
            {
                Stop();
            }
        }

        #region Local variables

        readonly BuzzIOWrite.BuzzIOWrite _buzz = null;
        int _activeController = 0;
        const int TimeAvailable = 5;
        bool _controllersActive = false;
        bool _timerActive = false;
        readonly Timer _timer;
        int _timeleft = 0;
        private int _lastActive;

        public int LastActive
        {
            get { return _lastActive; }
            set { _lastActive = value; }
        }

        #endregion

        readonly bool[] _players = new bool[5];
        readonly bool[] _pressed = new bool[5];
        readonly bool[] _used = new bool[5];

        private void HandleInput(int player)
        {
            if (_pressed[player - 1] || _used[player - 1] || _timerActive) return;

            _controllersActive = false;
            _showEvent.Publish(false);
            _pressed[player - 1] = true;

            for (var i = 0; i < _players.Length; i++)
            {
                _players[i] = i + 1 == player;
            }
            _standingsService.Players[player-1].IsActive = true;
            _standingsService.Players[player-1].IsAvailable = false;

            _used[player - 1] = true;
            _standingsService.TimeLeft = _timeleft;
            _timerActive = true;
            _timer.Enabled = true;
            ActiveController = player;
            TurnOnSingleLight(player);
        }

        public void ButtonDownEventHandler(object sender, KeyEventArgs e)
        {
            if (!_controllersActive || _timerActive) return;

            switch (e.Key)
            {
                case Key.Q:
                    HandleInput(1);
                    break;
                case Key.C:
                    HandleInput(2);
                    break;
                case Key.T:
                    HandleInput(3);
                    break;
                case Key.J:
                    HandleInput(4);
                    break;
                case Key.P:
                    HandleInput(5);
                    break;
            }
        }

        public void ButtonUpEventHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Q:
                    _pressed[0] = false;
                    break;
                case Key.C:
                    _pressed[1] = false;
                    break;
                case Key.T:
                    _pressed[2] = false;
                    break;
                case Key.J:
                    _pressed[3] = false;
                    break;
                case Key.P:
                    _pressed[4] = false;
                    break;
            }
        }

        private void OnTimerEnd(object sender, ElapsedEventArgs e)
        {
            _timeleft -= 1;
            _standingsService.TimeLeft = _timeleft;
            if (_timeleft > 0)
            {
                _timer.Enabled = false;
                _timer.Enabled = true;
            }
            else
            {
                Stop();

                Activate(true);
                _showEvent.Publish(true);
            }
        }

        private void Stop()
        {
            _timeleft = TimeAvailable;
            if (ActiveController>0)
                _standingsService.Players[ActiveController - 1].IsActive = false;
            ActiveController = 0;
            ActiveController = 0;
            _timer.Enabled = false;
            _timerActive = false;

            TurnOffAllLight();
        }

        private void TurnOnRemainingLight()
        {
            _activePlayerChangedEvent.Publish((_used[0] ? "x" : "-") + " " + (_used[1] ? "x" : "-") + " " + (_used[2] ? "x" : "-") + " " +
                (_used[3] ? "x" : "-") + " " + (_used[4] ? "x" : "-"));
            System.Diagnostics.Trace.WriteLine(string.Format("{0}, {1}, {2}, {3}"
                , !_used[0], !_used[1], !_used[2], !_used[3]));
            _buzz.Write(!_used[0], !_used[1], !_used[2], !_used[3]);
        }

        private void TurnOnSingleLight(int cnr)
        {
            _activePlayerChangedEvent.Publish((cnr == 1 ? "1" : "x") + " " + (cnr == 2 ? "2" : "x") + " " + (cnr == 3 ? "3" : "x") + " " +
                (cnr == 4 ? "4" : "x") + " " + (cnr == 5 ? "5" : "x"));

            System.Diagnostics.Trace.WriteLine(string.Format("{0}, {1}, {2}, {3}"
                , (cnr == 1 ? true : false), (cnr == 2 ? true : false), (cnr == 3 ? true : false), (cnr == 4 ? true : false)));
            _buzz.Write((cnr == 1 ? true : false), (cnr == 2 ? true : false), (cnr == 3 ? true : false), (cnr == 4 ? true : false));
        }

        private void TurnOffAllLight()
        {
            System.Diagnostics.Trace.WriteLine("false, false, false, false");
            _buzz.Write(false, false, false, false);
        }

        public int ActiveController
        {
            get { return _activeController; }
            set
            {
                _activeController = value;
                if (value != 0)
                    LastActive = value;
                RaisePropertyChanged(() => ActiveController);
            }
        }
    }
}
