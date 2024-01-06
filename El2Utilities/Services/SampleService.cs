
using System;

namespace El2Core.Services
{
    internal interface ISampleService
    {
        string GetCurrentDate();
    }
    internal class SampleService : ISampleService
    {
        public string GetCurrentDate()
        {
            throw new NotImplementedException();
        }
    }
}
