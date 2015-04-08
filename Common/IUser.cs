using System.Collections.Generic;

namespace Common
{
    public interface IUser
    {
        List<IDiginote> Diginotes { get; set; }
        string Name { get; }
        string Nickname { get; }
    }
}