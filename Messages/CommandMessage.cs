namespace Lieferliste_WPF.Messages
{
    public class CommandMessage
    {
        public CommandType Command { get; set; }
    }
    public enum CommandType
    {
        Insert,
        Edit,
        Delete,
        Commit,
        Refresh
    }

}
