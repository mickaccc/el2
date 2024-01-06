namespace El2Core.ViewModelBase
{
    public interface IViewModel
    {
        public string Title { get; }
        public bool HasChange { get; }
        public void Closing();
    }
}
