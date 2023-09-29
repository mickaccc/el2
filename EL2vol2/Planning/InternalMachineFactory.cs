using Lieferliste_WPF.ViewModels;

namespace Lieferliste_WPF.Planning
{

    class InternalMachineFactory : IMachineFactory
    {

        public IMachine createMachine()
        {
            IMachine mach = new InternalMachine();
            return mach;
        }
    }
}
