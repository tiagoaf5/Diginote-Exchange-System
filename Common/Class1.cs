using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Class1
    {
    }

    public interface IUser
    {
        bool LogUser(string nickname, string password);
        bool RegisterUser(string nickname, string password, string name);
    }
}
