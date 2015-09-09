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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Jbz.ViewModels;

namespace Jbz.Views
{
    /// <summary>
    /// Interaction logic for AdminArea.xaml
    /// </summary>
    public partial class AdminAreaView : Window
    {
        public AdminAreaView(AdminAreaViewModel adminAreaViewModel)
        {
            InitializeComponent();
            this.DataContext = adminAreaViewModel;

            this.PreviewKeyDown += ((AdminAreaViewModel)DataContext).InputService.ButtonDownEventHandler;
            this.PreviewKeyUp += ((AdminAreaViewModel)DataContext).InputService.ButtonUpEventHandler;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.PreviewKeyDown -= ((AdminAreaViewModel) DataContext).InputService.ButtonDownEventHandler;
            this.PreviewKeyUp -= ((AdminAreaViewModel) DataContext).InputService.ButtonUpEventHandler;
            base.OnClosing(e);
        }
    }
}
