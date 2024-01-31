using Prism.Commands;

namespace CompositeCommands.Core
{
    public interface IApplicationCommands
    {
        CompositeCommand ExplorerCommand { get; }
        CompositeCommand ArchivateCommand { get; }
        CompositeCommand OpenOrderCommand { get; }
        CompositeCommand OpenMachineCommand { get; }
        CompositeCommand OpenProjectOverViewCommand { get; }
        CompositeCommand CloseCommand { get; }
        CompositeCommand MachinePrintCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        private readonly CompositeCommand _explorerCommand = new();
        public CompositeCommand ExplorerCommand
        {
            get { return _explorerCommand; }
        }
        private CompositeCommand _archivateCommand = new();
        public CompositeCommand ArchivateCommand
        {
            get { return _archivateCommand; }
        }
        private CompositeCommand _deArchivateCommand = new();
        public CompositeCommand DeArchivateCommand
        {
            get { return _deArchivateCommand; }
        }
        private CompositeCommand _openOrderCommand = new();
        public CompositeCommand OpenOrderCommand
        {
            get { return _openOrderCommand; }
        }
        private CompositeCommand _openMachineCommand = new();
        public CompositeCommand OpenMachineCommand
        {
            get { return _openMachineCommand; }
        }
        private CompositeCommand _openProjectCommand = new();
        public CompositeCommand OpenProjectOverViewCommand
        {
            get { return _openProjectCommand; }
        }

        private CompositeCommand _closeCommand = new();
        public CompositeCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        private CompositeCommand _machinePrintCommand = new();
        public CompositeCommand MachinePrintCommand
        {
            get { return _machinePrintCommand; }
        }
    }
}
