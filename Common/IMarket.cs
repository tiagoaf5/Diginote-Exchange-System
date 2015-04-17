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

        void SuggestNewSharePrice(float newPrice, IUser user, bool sell, int quantity);
        void SuggestNewSharePrice(float newPrice, IUser user, IOrder order);

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

        List<IOrder> GetOrdersHistoy(IUser user);
    }
}
