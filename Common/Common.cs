using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting;

namespace Common
{
    public class Common
    {
    }

    public interface IUsers
    {
        IUser LogUser(string nickname, string password);
        IUser RegisterUser(string nickname, string password, string name);
    }

    public interface IUser
    {
        string Name { get; }
        string Nickname { get; }
    }

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
    public class Diginote
    {
        public String SerialNumber { get; set; }
        public int Value { get; private set; }

        public IUser User { get; set; }

        public Diginote(String serialNumber)
        {
            SerialNumber = serialNumber;
            Value = 1;
        }
    }
}
