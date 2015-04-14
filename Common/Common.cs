using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting;

namespace Common
{
    public class RemoteNew
    {
        private static Hashtable _types;

        private static void InitTypeTable()
        {
            _types = new Hashtable();
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                _types.Add(entry.ObjectType, entry);
        }

        public static object New(Type type)
        {
            if (_types == null)
                InitTypeTable();
            Debug.Assert(_types != null, "types != null"); //TODO: Added it
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)_types[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return RemotingServices.Connect(type, entry.ObjectUrl);
        }

    }


    public class EventsRepeater : MarshalByRefObject
    {
        public event ChangeDelegate ChangeEvent;
        public event UpdateLockingTimeDelegate UpdateLockingEvent;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void ChangeRepeater(float newPrice, ChangeOperation change)
        {
            if (ChangeEvent != null)
                ChangeEvent(newPrice, change);
        }

        public void LockingRepeater(int seconds)
        {
            if (UpdateLockingEvent != null)
                UpdateLockingEvent(seconds);
        }
    }
}
