using DbDiver.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbDiver.Modules.ViewModels
{
    public class ProgressViewModel : BindableBase
    {
        private string _progressLabelContent;
        private IEventAggregator _eventAggregtor;
        private int _progressBarMax = 100;
        private int _progressBarCurrentValue = 0;

        public int ProgressBarMax
        {
            get { return _progressBarMax; }
            set { SetProperty(ref _progressBarMax, value); }
        }

        public int ProgressBarCurrentValue
        {
            get { return _progressBarCurrentValue; }
            set { SetProperty(ref _progressBarCurrentValue, value); }
        }

        public string ProgressLabelContent
        {
            get { return _progressLabelContent; }
            set { SetProperty(ref _progressLabelContent, value); }
        }

        public ProgressViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregtor = eventAggregator;
            _eventAggregtor.GetEvent<DiveStartedEvent>().Subscribe(Started);
            _eventAggregtor.GetEvent<DiveFinishedEvent>().Subscribe(Stopped);
            _eventAggregtor.GetEvent<ItemProcessedEvent>().Subscribe(()=> { ProgressBarCurrentValue += 1; });
            _eventAggregtor.GetEvent<ScheduledDiveStartedEvent>().Subscribe(ScheduledStarted);
            _eventAggregtor.GetEvent<ScheduledDiveFinishedEvent>().Subscribe(ScheduledFinished);

        }

        public void Started(int maxValue)
        {
            ProgressLabelContent = "Started";
            ProgressBarMax = maxValue;
        }
        public void ScheduledStarted(int maxValue)
        {
            ProgressLabelContent = "Started scheduled dive with interval of 1 minute";
            ProgressBarMax = maxValue;
        }
        public void ScheduledFinished()
        {
            ProgressLabelContent = string.Empty;
            ProgressBarCurrentValue = 0;
        }

        public void Stopped()
        {
            ProgressLabelContent = "Search completed";
            ProgressBarCurrentValue = 0;
        }
    }
}
