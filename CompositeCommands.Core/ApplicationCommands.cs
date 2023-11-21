using Prism.Commands;

namespace CompositeCommands.Core
{
    public interface IApplicationCommands
    {
        CompositeCommand ExplorerCommand { get; }
        CompositeCommand ArchivateCommand { get; }
        CompositeCommand OpenOrderCommand { get; }
        CompositeCommand OpenMachineCommand { get; }
        CompositeCommand CloseCommand { get; }
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
        private CompositeCommand _closeCommand = new();
        public CompositeCommand CloseCommand
        {
            get { return _closeCommand; }
        }
    }
}
