using System;
using System.Collections.Generic;

namespace Common
{
    public interface IDiginote
    {
        String SerialNumber { get; }
        int Value { get; }

        IUser User { get; set; }
    }

    public enum SharePriceChange { Drop, Up };

    public delegate void SharePriceChangeDelegate(float newPrice, SharePriceChange change);

    public interface IMarket
    {
        event SharePriceChangeDelegate SharePriceEvent;
        List<IDiginote> BuyDiginotes(int quantity);
        int SellDiginotes(int quantity);
    }
}