using System.Diagnostics;

namespace Core
{
    public interface ITimeout
    {
        Stopwatch Watch { get; }
    }
}