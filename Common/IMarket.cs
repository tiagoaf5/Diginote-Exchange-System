using System.Collections.Generic;

namespace Common
{
    public interface IMarket
    {

        //Market
        event ChangeDelegate ChangeEvent; //general event used to let clients know prices drop
        List<IDiginote> BuyDiginotes(int quantity);
        int SellDiginotes(int quantity);

        void SuggestNewSharePrice(float newPrice);
    
        //USER
        IUser LogUser(string nickname, string password);
        IUser RegisterUser(string nickname, string password, string name);
    }
}
