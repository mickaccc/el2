namespace Lieferliste_WPF.Interfaces
{
    internal interface IProgressbarInfo
    {
        static double ProgressValue { get; set; }
        private static bool IsLoading { get; set; }
        void SetProgressIsBusy();
    }
}
