using System;
using System.Collections.Generic;
using Common;

namespace Server
{
    public class User : MarshalByRefObject, IUser
    {
        public List<Diginote> diginotes;

        public User(string name, string nickname)
        {
            Name = name;
            Nickname = nickname;
        }
        public string Name
        {
            get;
            private set;
        }

        public string Nickname { get; private set; }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}