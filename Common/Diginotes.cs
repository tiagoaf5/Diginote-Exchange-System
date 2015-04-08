using System;

namespace Common
{
    public interface IDiginote
    {
        String SerialNumber { get; }
        int Value { get; }

        IUser User { get; set; }
    }

    public enum ChangeOperation { ShareDrop, ShareUp };

    public delegate void ChangeDelegate(float newPrice, ChangeOperation change);

    public class ChangeEventRepeater : MarshalByRefObject
    {
        public event ChangeDelegate ChangeEvent;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Repeater(float newPrice, ChangeOperation change)
        {
            if (ChangeEvent != null)
                ChangeEvent(newPrice, change);
        }
    }
}