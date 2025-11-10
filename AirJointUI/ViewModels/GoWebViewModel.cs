using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.ViewModels
{
    public class GoWebViewModel : ViewModelBase
    {
        private string _reportUrl;
        public string ReportUrl
        {
            get { return _reportUrl; }
            set { _reportUrl = value; }
        }
    }
}
