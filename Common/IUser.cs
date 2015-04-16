using System.Collections.Generic;
using System.ComponentModel;

namespace Common
{
    public interface IUser : INotifyPropertyChanged
    {
        string Name { get; }
        string Nickname { get; }
        int IdUser { get;}
    }
}