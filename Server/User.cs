using System;
using System.Collections.Generic;
using Common;

namespace Server
{
    public class User : MarshalByRefObject, IUser
    {
        public List<IDiginote> Diginotes { get; set; }

        public User(string name, string nickname)
        {
            Name = name;
            Nickname = nickname;
        }

        public User(string name, string nickname, List<IDiginote> notes)
        {
            Name = name;
            Nickname = nickname;
            Diginotes = notes;
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