using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jbz.Services;
using Microsoft.Practices.Prism.Events;

namespace Jbz.ViewModels
{
    public class PictureQuestionWindowViewModel : MediaQuestionWindowViewModel
    {
        public PictureQuestionWindowViewModel(IEventAggregator eventAggregator, StandingsService standingsService, InputService inputService)
            : base(eventAggregator, standingsService, inputService)
        {
        }
    }
}
