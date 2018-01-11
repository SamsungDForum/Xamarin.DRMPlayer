namespace XamarinSamplePlayer
{
    internal class MediaUri
    {
        internal string uri { get; }
        internal bool usePlayready { get; }

        internal MediaUri(string s, bool playready = false)
        {
            uri = s;
            usePlayready = playready;
        }
    }
}
