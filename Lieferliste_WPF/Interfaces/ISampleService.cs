using System;

namespace Lieferliste_WPF.Interfaces
{
    internal interface ISampleService
    {
        string GetCurrentDate();
    }

    public class SampleService : ISampleService
    {
        public string GetCurrentDate() => DateTime.Now.ToLongDateString();
    }
}
