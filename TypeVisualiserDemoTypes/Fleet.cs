namespace TypeVisualiser.DemoTypes
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public class Fleet
    {
        /// <summary>
        /// test to ensure a simple method that immediately returns a new class is picked up by the consumption detection.
        /// </summary>
        /// <returns></returns>
        public static object GiveName()
        {
            var x = new Fleet();
            return new DivideByZeroException();
        }

        public Car CreateCar()
        {
            return new Car(Colors.BlueViolet);
        }

        public Fleet PartOf { get; set; }

        private Visibility Visibility { get; set; }
    }
}