using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Jbz.Events;
using Jbz.ViewModels;
using Microsoft.Practices.Prism.Events;

namespace Jbz.Views
{
    /// <summary>
    /// Interaction logic for SoundQuestionWindowView.xaml
    /// </summary>
    public partial class SoundQuestionWindowView : Window
    {
        SubscriptionToken token;
        CloseEvent closeEvent;
        
        public SoundQuestionWindowView(SoundQuestionWindowViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;

            closeEvent = viewModel.EventAggregator.GetEvent<CloseEvent>();
            token = closeEvent.Subscribe((o) =>
                                                                                   {
                                                                                       System.Diagnostics.Trace.WriteLine("Close stop event");
                                                                                       viewModel.Stop();
                                                                                       viewModel.Dispose();

                                                                                       this.Close();
                                                                                       this.Close();
                                                                                       this.Close();
                                                                                   }, ThreadOption.UIThread, true);
            System.Diagnostics.Trace.WriteLine(token);

            this.PreviewKeyDown += ((QuestionWindowViewModel)DataContext).InputService.ButtonDownEventHandler;
            this.PreviewKeyUp += ((QuestionWindowViewModel)DataContext).InputService.ButtonUpEventHandler;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            closeEvent.Unsubscribe(token);
            this.PreviewKeyDown -= ((QuestionWindowViewModel)DataContext).InputService.ButtonDownEventHandler;
            this.PreviewKeyUp -= ((QuestionWindowViewModel)DataContext).InputService.ButtonUpEventHandler;
            base.OnClosing(e);
        }
    }
}
