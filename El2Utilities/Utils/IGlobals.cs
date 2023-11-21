using El2Core.Models;

namespace El2Core.Utils
{
    public interface IGlobals
    {
        static string PC { get; }
        static User User { get; }
    }
}
