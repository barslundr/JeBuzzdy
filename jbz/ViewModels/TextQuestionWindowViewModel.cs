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
    public class TextQuestionWindowViewModel : QuestionWindowViewModel
    {
        public TextQuestionWindowViewModel(IEventAggregator eventAggregator, StandingsService standingsService, InputService inputService)
            : base(eventAggregator, standingsService, inputService)
        {
        }
    }
}
