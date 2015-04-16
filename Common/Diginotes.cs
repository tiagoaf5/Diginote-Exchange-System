using System;

namespace Common
{
    public interface IDiginote
    {
        String SerialNumber { get; }
        int Value { get; }

        IUser User { get; set; }
    }

    public enum ChangeOperation { ShareChange, UpdateInterface};

    public delegate void ChangeDelegate(ChangeOperation change);

    
}