using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Common;
using Server.Annotations;

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

        public int IdUser { get; internal set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}