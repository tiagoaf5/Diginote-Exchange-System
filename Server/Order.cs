using Common;
using System;

namespace Server
{
    class Order : MarshalByRefObject, IOrder, IComparable
    {
        public Order(OrderOptionEnum ot, int idO, int idU, int wanted, int satisfied, string date, bool closed = false)
        {
            Wanted = wanted;
            Satisfied = satisfied;
            Date = date;
            IdOrder = idO;
            IdUser = idU;
            OrderType = ot;
            Closed = closed;
        }

        public int Wanted { get; set; }
        public int Satisfied { get; set; }
        public int IdOrder { get; set; }
        public int IdUser { get; set; }

        public string Date { get; set; }

        public float SharePrice { get; set; }

        public OrderOptionEnum OrderType { get; set; }
        public bool Closed { get; set; }
        public int CompareTo(object obj)
        {
            Order o = (Order)obj;

            if (this.Date.Equals(o.Date))
                return IdOrder.CompareTo(o.IdOrder);

            return String.Compare(Date, o.Date, StringComparison.Ordinal);
        }
    }
}
