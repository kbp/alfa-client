using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ALFA_Client.Preloader
{
    class Preloader
    {
        Thread _thread = new Thread(new ThreadStart(ShowWindows));

        private void ShowWindows()
        {
            _preloaderWindow.ShowDialog();
        }

        PreloaderWindow _preloaderWindow = new PreloaderWindow();
    }
}
