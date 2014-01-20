using System.ComponentModel;

namespace TypeVisualiser.DemoTypes
{
    using System;

    public interface IWearAndTear : INotifyPropertyChanged 
    {
        event EventHandler ComponentWornOut;
        void RequiresService();
    }
}