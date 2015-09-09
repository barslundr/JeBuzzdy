using System;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace Jbz.ViewModels
{
    /// <summary>
    /// The view model for dialogs
    /// </summary>
    public abstract class DialogViewModelBase : NotificationObject
    {
        private ICommand _closeCommand;
        private ICommand _okCommand;

        /// <summary>
        /// Kommando til at lukke diagogboxen med DialogResult == false
        /// </summary>
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new DelegateCommand(OnCloseCommandExecute)); }
        }

        /// <summary>
        /// Kommando til at lukke dialogboxen med DialogResult == true
        /// </summary>
        public ICommand OkCommand
        {
            get { return _okCommand ?? (_okCommand = new DelegateCommand(OnOkCommandExecute)); }
        }

        /// <summary>
        /// Event der sendes ved anvendelse af CloseCommand
        /// </summary>
        public event EventHandler ExecuteClose;

        /// <summary>
        /// Event der sendes ved anvendelse af OkCommand
        /// </summary>
        public event EventHandler ExecuteOk;

        /// <summary>
        /// Kaldes ved CloseCommand
        /// </summary>
        protected virtual void OnCloseCommandExecute()
        {
            EventHandler handler = this.ExecuteClose;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Kaldes ved kalde kald af OkKommando
        /// </summary>
        protected virtual void OnOkCommandExecute()
        {
            EventHandler handler = this.ExecuteOk;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}


