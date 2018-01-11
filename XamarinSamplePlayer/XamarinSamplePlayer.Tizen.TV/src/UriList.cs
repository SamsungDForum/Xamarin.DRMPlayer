using System;
using System.Collections;
using System.Collections.Generic;

namespace XamarinSamplePlayer
{
    internal class UriList
    {
        private LinkedList<MediaUri> uriList;
        private LinkedListNode<MediaUri> currentTrack;

        private string[] defaultUris =
        {
        };

        private string[] defaultPlayreadyUris =
        {
            "http://playready.directtaps.net/smoothstreaming/SSWSS720H264PR/SuperSpeedway_720.ism/Manifest"
        };

        internal UriList()
        {
            uriList = new LinkedList<MediaUri>();

            foreach(string uri in defaultUris)
            {
                uriList.AddLast(new MediaUri(uri));
            }
            foreach(string uri in defaultPlayreadyUris)
            {
                uriList.AddLast(new MediaUri(uri, true));
            }
            currentTrack = uriList.First;
        }

        internal UriList(string[] uris, string[] playready_uris = null)
        {
            uriList = new LinkedList<MediaUri>();

            foreach (string uri in uris)
            {
                uriList.AddLast(new MediaUri(uri));
            }
            foreach (string uri in playready_uris)
            {
                uriList.AddLast(new MediaUri(uri, true));
            }
            currentTrack = uriList.First;
        }

        internal void Add(string uri, bool playready = false)
        {
            uriList.AddLast(new MediaUri(uri, playready));
        }

        internal MediaUri GetCurrentTrack()
        {
            try
            {
                if (currentTrack != null)
                {
                    return currentTrack.Value;
                }
                return null;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        internal bool MoveToNextTrack()
        {
            
            try
            {
                if(currentTrack.Next != null)
                {
                    currentTrack = currentTrack.Next;
                    return true;
                }
                return false;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
    }
}
