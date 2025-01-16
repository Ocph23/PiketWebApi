using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile
{
    public class BaseNotify : ObservableObject
    {

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }


    }
}
