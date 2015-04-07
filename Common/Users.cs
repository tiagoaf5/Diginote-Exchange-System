using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
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
}
