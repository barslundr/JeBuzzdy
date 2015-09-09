using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jbz.Events;
using Jbz.Services;
using Microsoft.Practices.Prism.Events;

namespace Jbz.ViewModels
{
    public class VideoQuestionWindowViewModel : MediaQuestionWindowViewModel, IDisposable
    {
        private SubscriptionToken _soundToken;
        private SubscriptionToken _stopToken;
        private readonly CloseEvent _closeEvent;

        public VideoQuestionWindowViewModel(IEventAggregator eventAggregator, StandingsService standingsService, InputService inputService)
            : base(eventAggregator, standingsService, inputService)
        {
            _soundToken = ShowEvent.Subscribe(PauseStart);
            _closeEvent = eventAggregator.GetEvent<CloseEvent>();
            _stopToken = _closeEvent.Subscribe(o => Dispose());
        }

        private void PauseStart(bool activate)
        {
            if (activate)
            {
                Play();
            }
            else
            {
                Pause();
            }
        }

        public event EventHandler PlayRequested;

        private void Play()
        {
            if (this.PlayRequested != null)
            {
                this.PlayRequested(this, EventArgs.Empty);
            }
        }

        public event EventHandler PauseRequested;

        private void Pause()
        {
            if (this.PauseRequested != null)
            {
                this.PauseRequested(this, EventArgs.Empty);
            }
        }

        public event EventHandler StopRequested;

        public void Stop()
        {
            if (this.StopRequested != null)
            {
                this.StopRequested(this, EventArgs.Empty);
            }
        }
        
        private bool _disposed;
        
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
                    Stop();
                    _closeEvent.Unsubscribe(_stopToken);
                    ShowEvent.Unsubscribe(_soundToken);
                }
                base.Dispose();
                _disposed = true;
            }
        }
        #endregion
    }
}
