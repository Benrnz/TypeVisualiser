namespace TypeVisualiser.DemoTypes
{
    public interface ITransportProvider
    {
        int NumberOfPassengers { get; set; }
        bool IsOperational { get; }

        double Move(double requestedDistance, string destination);
    }
}