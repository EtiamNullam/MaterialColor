using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReloadColors.ViewModels
{
    public class ReloadColorsViewModel : BindableBase
    {
        public ReloadColorsViewModel(WCF.ColorReloaderClient reloaderClient)
        {
            _reloaderClient = reloaderClient;

            ReloadCommand = new DelegateCommand(ReloadColors);
        }

        private WCF.ColorReloaderClient _reloaderClient;

        public DelegateCommand ReloadCommand { get; private set; }

        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        private string _status;

        public void ReloadColors()
        {
            try
            {
                _reloaderClient.SendReload();
            }
            catch (Exception e)
            {
                Status = $"Send failed\n" + e.Message + "\n" + e.StackTrace;
            }
        }
    }
}
