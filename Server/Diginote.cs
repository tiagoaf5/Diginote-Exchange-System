using System;
using Common;

namespace Server
{
    public class Diginote : MarshalByRefObject, IDiginote
    {
        public string SerialNumber { get; set; }
        public int Value { get; private set; }

        public IUser User { get; set; }

        public Diginote(string serialNumber)
        {
            SerialNumber = serialNumber;
            Value = 1;
        }

        public Diginote(string serialNumber, IUser u)
        {
            SerialNumber = serialNumber;
            User = u;
            Value = 1;
        }
    }
}