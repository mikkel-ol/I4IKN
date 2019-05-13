using System.Diagnostics;

namespace Core
{
    public class Timeout : ITimeout
    {
        public Stopwatch Watch { get; private set; }

        public Timeout()
        {
            this.Watch = new Stopwatch();
        }
    }
}