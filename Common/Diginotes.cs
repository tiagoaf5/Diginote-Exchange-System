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

    
}