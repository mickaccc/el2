namespace Lieferliste_WPF.Planning
{
    class ParkMachineFactory : IMachineFactory
    {
        public IMachine createMachine()
        {
            IMachine m = new ParkMachine();
            return m;
        }
    }
}
