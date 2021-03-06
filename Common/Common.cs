﻿using System;
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
            Debug.Assert(_types != null, "types != null"); // Added it
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)_types[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return RemotingServices.Connect(type, entry.ObjectUrl);
        }

    }


    public class EventsRepeater : MarshalByRefObject
    {
        public event ChangeDelegate ChangeEvent;
        public event UpdateTimerLockingDelegate UpdateLockingEvent;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void ChangeRepeater(ChangeOperation change,int value)
        {
            if (ChangeEvent != null)
                ChangeEvent(change,value);
        }

        public void LockingRepeater(bool start)
        {
            if (UpdateLockingEvent != null)
                UpdateLockingEvent(start);
        }
    }

    public static class Constants
    {
        public static int TimerSeconds = 60;
    }
}
