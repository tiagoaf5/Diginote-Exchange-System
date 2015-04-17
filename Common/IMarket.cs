using System.Collections;
using System.Collections.Generic;

namespace Common
{

    public delegate void UpdateTimerLockingDelegate(bool start);

    public interface IMarket
    {


        //Market
        float SharePrice { get; }
        int CountDown { get; }

        //general event used to let clients know prices drop
        event ChangeDelegate ChangeEvent;
        //general event that lets clients know that market locked time
        event UpdateTimerLockingDelegate UpdateLockingEvent;
        int BuyDiginotes(int quantity, IUser user);
        int SellDiginotes(int quantity, IUser user);

        IOrder GetUserPendingOrder(IUser user);

        void SuggestNewSharePrice(float newPrice, IUser user, bool Sell, int quantity);

        //USER
        IUser LogUser(string nickname, string password, string address);
        IUser RegisterUser(string nickname, string password, string name, string address);

        ArrayList GetSharePricesList();
        void Logout(IUser user);
        List<IDiginote> GetUserDiginotes(IUser user);

        int GetNumberOfDemmandingDiginotes();

        int GetNumberOfAvailableDiginotes();

        void KeepOrderOn(IOrder order);

        void RevokeOrder(IOrder order);
    }
}
