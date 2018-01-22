using Xamarin.Forms;
using System.Timers;
using System.Collections.Generic;

namespace XamarinSamplePlayer
{
    internal class PositionBar : UISynchronizer
    {
        private Timer timer;
        private Slider slider;
        private IList<View> layout;
        private PlayerControl playerControl;
        private bool activated = false;

        internal PositionBar(IList<View> layout, PlayerControl playerControl)
        {
            this.layout = layout;
            this.playerControl = playerControl;
            slider = new Slider();
            timer = new Timer(100);
            timer.Elapsed += TimerTickUpdater;
        }

        internal void Activate(uint max)
        {
            Tizen.Log.Info("KEY", "Max value: " + max);
            if (activated == false && max > 1)
            {
                activated = true;
                timer.Start();
                slider.Maximum = max;
                slider.Value = playerControl.GetCurrentPosition();
                layout.Add(slider);
            }
        }

        internal void Deactivate()
        {
            if (activated == true)
            {
                activated = false;
                layout.Remove(slider);
                timer.Stop();
            }
        }

        private void TimerTickUpdater(object s, ElapsedEventArgs e)
        {
            var current = (uint)playerControl.GetCurrentPosition();
            Update(current);
        }

        override protected void SynchronizationUpdate(object o)
        {
            var current = (uint)o;
            Tizen.Log.Info("KEY", "Current: " + current);
            slider.Value = current;
        }
    }
}
