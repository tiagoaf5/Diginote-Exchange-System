using Common;
using System;

namespace Server
{
    class Order : MarshalByRefObject, IOrder
    {
        public Order(int idO, int idU, int wanted, int satisfied)
        {
            Wanted = wanted;
            Satisfied = satisfied;
            IdOrder = idO;
            IdUser = idU;

        }

        public int Wanted { get; set; }
        public int Satisfied { get; set; }
        public int IdOrder { get; set; }
        public int IdUser { get; set; }

    }
}
