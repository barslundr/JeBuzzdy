using System;
using System.Media;
using System.IO;
using System.Collections;
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
using System.Windows.Threading;
using System.Diagnostics;
using Jbz.Services;
using Microsoft.Practices.Prism.Events;
using Jbz.Events;
using Jbz.ViewModels;
using Jbz.Views;

namespace Jbz
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private AdminAreaView _adminArea;
        private AdminAreaViewModel _adminAreaViewModel;
        private QuestionGridViewModel _questionGridViewModel;
        private RoundService _roundService;
        private StandingsService _standingsService;
        private InputService _inputService;

        public Window1()
        {
            InitializeComponent();
            var eventAggregator = new EventAggregator();

            _standingsService = new StandingsService(5);
            _roundService = new RoundService();
            //var names = new List<string> {"Emil", "CJ", "Rune", "Andreas", "Kristoffer"};
            var counter = 1;
            foreach (var p in _standingsService.Players)
            {
                p.Name = "P" + counter++;
                //p.Name = names[counter - 1];
                //p.Points = counter*100;
                //counter++;
            }
            _inputService = new InputService(eventAggregator, _standingsService);

            _questionGridViewModel = new QuestionGridViewModel(eventAggregator, _roundService, _standingsService);

            _adminAreaViewModel = new AdminAreaViewModel(eventAggregator, _questionGridViewModel, _roundService, _standingsService, _inputService);
            _adminArea = new AdminAreaView(_adminAreaViewModel);
            _adminArea.Show();
            var mainViewModel = new MainViewModel(this, eventAggregator, _roundService, _standingsService, _inputService);
            MainView.DataContext = mainViewModel;


            this.PreviewKeyDown += _inputService.ButtonDownEventHandler;
            this.PreviewKeyUp += _inputService.ButtonUpEventHandler;
            
            new System.Threading.Tasks.TaskFactory().StartNew(() => { System.Threading.Thread.Sleep(100);
                                                                        UIHelperService.ExecuteUIAction(() =>
                                                                                                        this.WindowState
                                                                                                        =
                                                                                                        WindowState.
                                                                                                            Minimized);
            });
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.PreviewKeyDown -= _inputService.ButtonDownEventHandler;
            this.PreviewKeyUp -= _inputService.ButtonUpEventHandler;
            base.OnClosing(e);
        }
    }
}
