using Tizen.Applications;
using Xamarin.Forms;
using System.Threading;

namespace XamarinSamplePlayer
{
    internal static class MessageBox
    {
        private static SynchronizationContext uiContext;
        private static Page parent;
        static internal void SetParent(Page parent)
        {
            MessageBox.parent = parent;
            // TizenSynchronizationContext
            if (TizenSynchronizationContext.Current == null)
            {
                TizenSynchronizationContext.Initialize();
            }
            uiContext = TizenSynchronizationContext.Current;
        }

        internal static void New(string message)
        {
            if (uiContext != TizenSynchronizationContext.Current)
            {
                uiContext.Post(SynchronizationUpdate, message);
            }
            else
            {
                SynchronizationUpdate(message);
            }
        }

        private static void SynchronizationUpdate(object o)
        {
            string message = o as string;
            parent.DisplayAlert("Alert", message, "OK");
        }
    }
}
