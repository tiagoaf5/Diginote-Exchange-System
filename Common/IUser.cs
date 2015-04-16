using System.Collections.Generic;
using System.ComponentModel;

namespace Common
{
    public interface IUser
    {
        string Name { get; }
        string Nickname { get; }
        int IdUser { get;}
        
    }
}