using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jbz.Models;
using Microsoft.Practices.Prism.ViewModel;

namespace Jbz.Services
{
    public class RoundService : NotificationObject
    {
        public RoundService()
        {
            LoadData();
        }

        private void LoadData()
        {
            Round1 = new Round();
            Round1.Load(1);

            Round2 = new Round();
            Round2.Load(2);

            Final = new Round();
            Final.Load(3);

            CurrentRound = Round1;
        }

        private Round _round1;
        private Round _round2;

        public void ChangeRound(int round)
        {
            switch (round)
            {
                case 1:
                    CurrentRound = Round1;
                    break;
                case 2:
                    CurrentRound = Round2;
                    break;
                case 3:
                    CurrentRound = Final;
                    break;
                default:
                    CurrentRound = new Round();
                    break;
            }
        }

        public Round Round1
        {
            get { return _round1; }
            set { _round1 = value; }
        }

        public Round Round2
        {
            get { return _round2; }
            set { _round2 = value; }
        }

        public Round Final
        {
            get { return _final; }
            set { _final = value; }
        }

        private Round _final;

        private Round _currentRound;

        public Round CurrentRound
        {
            get { return _currentRound; }
            set
            {
                _currentRound = value;
                RaisePropertyChanged(() => CurrentRound);
            }
        }
    }
}
