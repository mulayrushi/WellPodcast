using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;

namespace Well_Podcast
{
    sealed partial class App : Application
    {
        bool isInBackgroundMode;

        partial void Construct()
        {
            MemoryManager.AppMemoryUsageLimitChanging += MemoryManager_AppMemoryUsageLimitChanging;
            
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_AppMemoryUsageIncreased;
            
            EnteredBackground += App_EnteredBackground;
            LeavingBackground += App_LeavingBackground;
            
            Suspending += App_Suspending;
            Resuming += App_Resuming;
        }

        private void MemoryManager_AppMemoryUsageLimitChanging(object sender, AppMemoryUsageLimitChangingEventArgs e)
        {

        }

        private void MemoryManager_AppMemoryUsageIncreased(object sender, object e)
        {

        }

        private void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {

        }

        private void App_LeavingBackground(object sender, LeavingBackgroundEventArgs e)
        {

        }

        private void App_Suspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // Optional: Save application state and stop any background activity
            //ShowToast("Suspending");
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            //ShowToast("Resuming");
        }
    }
}
