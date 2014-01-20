using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Media;

namespace TypeVisualiser.DemoTypes
{
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Test Code only")]
    [GeneratedCode("Test Code", "1.0")]
    public class Car : Vehicle, ITransportProvider, IWearAndTear, ICloneable
    {
        // ReSharper disable UnusedMember.Local
        private Color color1;
        private Color color2;
        private Color color3;
        private Color color4;
        private Color color5;
        private SqlCommand command;
        private KeyValuePair<string, IWearAndTear> gearbox;
        // ReSharper restore UnusedMember.Local

        public Car(Color color) : this()
        {
            Color = color;
        }

        private Car()
        {
            Age = 1;
            Engine = new CombustionEngine();
            NumberOfDoors = 5;
            this.color1 = Colors.Beige;
            this.color2 = Colors.Cyan;
            this.color3 = Colors.Crimson;
            this.color4 = Colors.DarkGoldenrod;
            this.color5 = Colors.DarkKhaki;
            this.gearbox = new KeyValuePair<string, IWearAndTear>("Manual 4 speed", null);
        }

        public event EventHandler ComponentWornOut;
        public event PropertyChangedEventHandler PropertyChanged;

        public int Age { get; set; }
        public Color Color { get; set; }
        public IDbConnection Connection { get; private set; }

        public bool IsOperational { get; set; }
        public int NumberOfPassengers { get; set; }
        protected double Diameter { get; set; }

        public override bool SelfTest()
        {
            return true;
        }

        public object Clone()
        {
            PropertyChanged(this, new PropertyChangedEventArgs(""));
            throw new NotImplementedException();
        }

        public double Move(double requestedDistance, string destination)
        {
            double d = destination.Length + 34.56;
            return requestedDistance * d / 1;
        }

        public void RequiresService()
        {
            // ReSharper disable UnusedVariable
            var x = new SqlConnection();
            // ReSharper restore UnusedVariable
            this.command = new SqlCommand();
            ComponentWornOut(this, Empty);
            StaticTestClass.DoWork();
        }
    }
}