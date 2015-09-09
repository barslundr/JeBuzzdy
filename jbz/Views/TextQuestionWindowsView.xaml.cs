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

namespace Jbz.Views
{
    /// <summary>
    /// Interaction logic for QuestionWindowsView.xaml
    /// </summary>
    public partial class TextQuestionWindowView : Window
    {
        public TextQuestionWindowView(TextQuestionWindowViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;

            viewModel.EventAggregator.GetEvent<CloseEvent>().Subscribe((o) => this.Close());

            this.PreviewKeyDown += ((QuestionWindowViewModel)DataContext).InputService.ButtonDownEventHandler;
            this.PreviewKeyUp += ((QuestionWindowViewModel)DataContext).InputService.ButtonUpEventHandler;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.PreviewKeyDown -= ((QuestionWindowViewModel)DataContext).InputService.ButtonDownEventHandler;
            this.PreviewKeyUp -= ((QuestionWindowViewModel)DataContext).InputService.ButtonUpEventHandler;
            base.OnClosing(e);
        }
    }
}
