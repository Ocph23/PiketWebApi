using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicketMobile
{
    internal class BaseNotify :ObservableObject
    {


		private bool isBusy;

		public bool IsBusy
		{
			get { return isBusy; }
			set {SetProperty(ref isBusy , value); }
		}

	}
}
