using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jbz.Events;
using Jbz.Services;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;

namespace Jbz.ViewModels
{
    public class QuestionWindowViewModel : NotificationObject
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly StandingsService _standingsService;
        private readonly InputService _inputService;

        public QuestionWindowViewModel(IEventAggregator eventAggregator, StandingsService standingsService, InputService inputService)
        {
            _eventAggregator = eventAggregator;
            _standingsService = standingsService;
            _inputService = inputService;
        }

        public InputService InputService
        {
            get { return _inputService; }
        }

        public StandingsService StandingsService
        {
            get { return _standingsService; }
        }

        public IEventAggregator EventAggregator
        {
            get { return _eventAggregator; }
        }
    }

    public class MediaQuestionWindowViewModel : QuestionWindowViewModel, IDisposable
    {
        public MediaQuestionWindowViewModel(IEventAggregator eventAggregator, StandingsService standingsService, InputService inputService)
            : base(eventAggregator, standingsService, inputService)
        {
            ShowEvent = EventAggregator.GetEvent<ShowEvent>();
            _activeToken = ShowEvent.Subscribe((o) => IsActive = o);
        }

        SubscriptionToken _activeToken;
        private ShowEvent _showEvent;

        public ShowEvent ShowEvent
        {
            get { return _showEvent; }
            set { _showEvent = value; }
        }

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged(() => IsActive);
            }
        }

        private string _path;
        private bool _disposed;

        public string Path
        {
            get { return _path; }
            set
            {
                _path = System.IO.Path.GetFullPath(System.IO.Path.Combine(@"Data", value));
                RaisePropertyChanged(() => Path);
            }
        }
        #region Dispose
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to dispose all managed and unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    ShowEvent.Unsubscribe(_activeToken);
                    ShowEvent = null;
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
