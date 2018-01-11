using Xamarin.Forms;
using System.Collections.Generic;

namespace XamarinSamplePlayer
{
    internal class LoadingIcon : UISynchronizer, IUIUpdate
    {
        private ActivityIndicator indicator;
        private bool activated = false;
        IList<View> layout;

        internal LoadingIcon(IList<View> layout) : base()
        {
            this.layout = layout;
            indicator = new ActivityIndicator();
            indicator.IsRunning = false;
        }

        override protected void SynchronizationActivate(object o)
        {
            if (activated == false)
            {
                activated = true;
                indicator.IsRunning = true;
                layout.Add(indicator);
            }
        }

        override protected void SynchronizationDeactivate(object o)
        {
            if (activated == true)
            {
                activated = false;
                indicator.IsRunning = false;
                layout.Remove(indicator);
            }
        }

        override public void Update(object o)
        {
            bool activated = (bool)o;

            if (activated == true)
            {
                Activate(o);
            }
            else
            {
                Deactivate(o);
            }
        }
    }
}
