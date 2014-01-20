using System;

namespace TypeVisualiser.DemoTypes
{
    internal static class StaticTestClass
    {
        public static void DoWork()
        {
            // TODO This is a bug - Analysis does not pick up this static usage.
            Console.WriteLine(Vehicle.Code);
        }

        public static object Current { get; set; }
    }
}
