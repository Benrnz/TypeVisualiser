namespace TypeVisualiser.DemoTypes
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    [GeneratedCode("Test Code", "1.0")]
    public class Transmission<T> : IDisposable where T : Engine 
    {
        public Transmission(T engine)
        {
            Engine = engine;
        }

        public T Engine { get; private set; }

        protected virtual bool ChangeUp(out bool something, out DateTime completed)
        {
            something = true;
            completed = DateTime.Now;
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    [GeneratedCode("Test Code", "1.0")]
    public class TiptronicTransmission<T> : Transmission<T> where T : Engine
    {
        private List<int> someInts;
        private IDictionary<int, Tuple<string, List<int>>> cogs;
        private Stack<T> engineStack;
        private int alpha = 3;
        private int beta = 1;

        public TiptronicTransmission(List<T> engines, List<object> ratioInfo) : base(engines[0])
        {
            engineStack = new Stack<T>(beta);
            someInts = new List<int> { alpha, beta, 5, 8, 12 };
            cogs = new Dictionary<int, Tuple<string, List<int>>>();
        }

        public T ActivateSecondary(ref string name)
        {
            var x = new List<int> { beta * alpha, beta, alpha };
            Thread.Sleep(someInts.First());
            return engineStack.ElementAt(x.First());
        }

        protected override bool ChangeUp(out bool something, out DateTime completed)
        {
            var result = base.ChangeUp(out something, out completed);
            return !result || cogs.First().Value.Item2.First() % alpha == beta;
        }

    }
}
