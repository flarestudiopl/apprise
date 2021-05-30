namespace FlareStudio.Apprise.Domain
{
    internal enum MessageState : byte
    {
        Enqueued,
        Sent,
        Retrying,
        Failed
    }
}
