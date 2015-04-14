using System.Collections.Generic;

namespace Common
{

    public delegate void UpdateLockingTimeDelegate(int seconds);

    public interface IMarket
    {


        //Market
        float SharePrice { get; }
        //general event used to let clients know prices drop
        event ChangeDelegate ChangeEvent; 
        //general event that lets clients know that market locked time
        event UpdateLockingTimeDelegate UpdateLockingEvent;
        List<IDiginote> BuyDiginotes(int quantity);
        int SellDiginotes(int quantity, IUser user);

        void SuggestNewSharePrice(float newPrice, IUser user);

        //USER
        IUser LogUser(string nickname, string password);
        IUser RegisterUser(string nickname, string password, string name);
    }
}
