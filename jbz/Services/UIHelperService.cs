using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jbz.Services
{
    public static class UIHelperService
    {
        public static void ExecuteUIAction(Action action)
        {
            if (System.Windows.Application.Current != null)
            {
                if (System.Windows.Application.Current.CheckAccess())
                {
                    action();
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(action);
                }
            }
        }
    }
}
