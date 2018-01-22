using System;
using System.Threading;
using Tizen.Applications;

namespace XamarinSamplePlayer
{
    internal abstract class UISynchronizer
    {
        private static SynchronizationContext uiContext;

        protected UISynchronizer()
        {
            if (TizenSynchronizationContext.Current == null)
            {
                TizenSynchronizationContext.Initialize();
            }
            uiContext = TizenSynchronizationContext.Current;
        }

        virtual protected void SynchronizationActivate(object o)
        {
            throw new NotImplementedException();
        }

        virtual protected void SynchronizationDeactivate(object o)
        {
            throw new NotImplementedException();
        }

        virtual protected void SynchronizationUpdate(object o)
        {
            throw new NotImplementedException();
        }

        virtual internal void Activate(object o)
        {
            if (uiContext != TizenSynchronizationContext.Current)
            {
                uiContext.Post(SynchronizationActivate, o);
            }
            else
            {
                SynchronizationActivate(o);
            }
        }

        virtual internal void Deactivate(object o)
        {
            if (uiContext != TizenSynchronizationContext.Current)
            {
                uiContext.Post(SynchronizationDeactivate, o);
            }
            else
            {
                SynchronizationDeactivate(o);
            }
        }

        virtual public void Update(object o)
        {
            if (uiContext != TizenSynchronizationContext.Current)
            {
                uiContext.Post(SynchronizationUpdate, o);
            }
            else
            {
                SynchronizationUpdate(o);
            }
        }
    }
}
