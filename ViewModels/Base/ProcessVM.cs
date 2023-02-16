using Lieferliste_WPF.Entities;

namespace Lieferliste_WPF.ViewModels.Base
{
    public class ProcessVM : VMBase
    {
        public lieferliste TheProcess { get; set; }

        public ProcessVM()
        {
            TheProcess = new lieferliste();
            TheProcess.ausgebl = false;

        }
    }
}
