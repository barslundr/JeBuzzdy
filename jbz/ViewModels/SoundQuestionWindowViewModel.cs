using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows.Media;
using Jbz.Events;
using Jbz.Services;
using Microsoft.Practices.Prism.Events;

namespace Jbz.ViewModels
{
    public class SoundQuestionWindowViewModel : MediaQuestionWindowViewModel, IDisposable
    {
        private SubscriptionToken _soundToken;
        private SubscriptionToken _stopToken;
        readonly CloseEvent _closeEvent;

        public SoundQuestionWindowViewModel(IEventAggregator eventAggregator, StandingsService standingsService, InputService inputService)
            : base(eventAggregator, standingsService, inputService)
        {
            _soundToken = ShowEvent.Subscribe(PauseStart);
            _closeEvent = eventAggregator.GetEvent<CloseEvent>();
            _stopToken = _closeEvent.Subscribe((o) => Dispose());
        }

        private void PauseStart(bool activate)
        {
            UIHelperService.ExecuteUIAction(() =>
            {
                if (_soundPlayer == null)
                {
                    _soundPlayer = new MediaPlayer();
                    _soundPlayer.Open(new Uri(Path));
                    _soundPlayer.MediaEnded += MediaEnded;
                }

                if (activate)
                {
                    _soundPlayer.Play();
                }
                else
                {
                    _soundPlayer.Pause();
                }
            });
        }

        private void MediaEnded(object sender, EventArgs e)
        {
            UIHelperService.ExecuteUIAction(() =>
            {
                _soundPlayer.Stop();
                _soundPlayer.Position = TimeSpan.FromSeconds(0);
            });
        }

        private MediaPlayer _soundPlayer;

        private bool _disposed;
       
        public void Stop()
        {
            UIHelperService.ExecuteUIAction(() =>
            {
                if (_soundPlayer == null) return;

                _soundPlayer.MediaEnded -= MediaEnded;
                _soundPlayer.Stop();
                _soundPlayer.Close();
                _soundPlayer = null;
            });
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
                Stop();
                if (disposing)
                {
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
