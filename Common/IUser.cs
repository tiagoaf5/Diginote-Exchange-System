using System.Collections.Generic;
using System.ComponentModel;

namespace Common
{
    public interface IUser : INotifyPropertyChanged
    {
        List<IDiginote> Diginotes { get; set; }
        string Name { get; }
        string Nickname { get; }
        int IdUser { get;}
    }
}