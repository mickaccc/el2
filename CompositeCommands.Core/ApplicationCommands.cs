using System;
using Prism.Commands;

namespace CompositeCommands.Core
{
    public interface IApplicationCommands
    {
        CompositeCommand ExplorerCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        private CompositeCommand _explorerCommand = new CompositeCommand();
        public CompositeCommand ExplorerCommand
        {
            get { return _explorerCommand; }
        }
    }
}
