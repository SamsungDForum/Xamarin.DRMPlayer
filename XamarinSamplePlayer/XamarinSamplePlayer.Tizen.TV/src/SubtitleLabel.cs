using Xamarin.Forms;
using System.Collections.Generic;

namespace XamarinSamplePlayer
{
    internal class SubtitleLabel : UISynchronizer, IUIUpdate
    {
        Label label;

        internal SubtitleLabel(IList<View> layout) : base()
        {
            label = new Label
            {
                HorizontalTextAlignment = TextAlignment.Start,
                TextColor = Color.White,
            };
            layout.Add(label);
        }

        override protected void SynchronizationUpdate(object o)
        {
            label.Text = o as string;
        }

        internal void Clear()
        {
            label.Text = "";
        }
    }
}
