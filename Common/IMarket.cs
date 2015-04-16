using System.Collections;
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
        int BuyDiginotes(int quantity, IUser user);
        int SellDiginotes(int quantity, IUser user);

        void SuggestNewSharePrice(float newPrice, IUser user, bool sell, int quantity);

        //USER
        IUser LogUser(string nickname, string password);
        IUser RegisterUser(string nickname, string password, string name);

        ArrayList GetSharePricesList();
        void Logout(IUser user);
        List<IDiginote> GetUserDiginotes(IUser user);
    }
}
