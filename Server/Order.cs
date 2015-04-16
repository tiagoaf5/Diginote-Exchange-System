using Common;
using System;

namespace Server
{
    class Order : MarshalByRefObject, IOrder
    {
        public Order(OrderOptionEnum ot, int idO, int idU, int wanted, int satisfied, bool closed = false)
        {
            Wanted = wanted;
            Satisfied = satisfied;
            IdOrder = idO;
            IdUser = idU;
            OrderType = ot;
            Closed = closed;
        }

        public int Wanted { get; set; }
        public int Satisfied { get; set; }
        public int IdOrder { get; set; }
        public int IdUser { get; set; }
        public OrderOptionEnum OrderType { get; set; }
        public bool Closed { get; set; }
    }
}
