using System;
using Prism.Commands;

namespace CompositeCommands.Core
{
    public interface IApplicationCommands
    {
        CompositeCommand ExplorerCommand { get; }
        CompositeCommand ArchivateCommand { get; }
        CompositeCommand OpenOrderCommand { get; }
        CompositeCommand OpenMachineCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        private CompositeCommand _explorerCommand = new CompositeCommand();
        public CompositeCommand ExplorerCommand
        {
            get { return _explorerCommand; }
        }
        private CompositeCommand _archivateCommand = new CompositeCommand();
        public CompositeCommand ArchivateCommand
        {
            get { return _archivateCommand; }
        }
        private CompositeCommand _openOrderCommand = new CompositeCommand();
        public CompositeCommand OpenOrderCommand
        {
            get { return _openOrderCommand; }
        }
        private CompositeCommand _openMachineCommand = new CompositeCommand();
        public CompositeCommand OpenMachineCommand
        {
            get { return _openMachineCommand; }
        }

    }
}
