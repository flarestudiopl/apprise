namespace FlareStudio.Apprise.Domain
{
    public enum MessageState : byte
    {
        Enqueued,
        Sent,
        Retrying,
        Failed
    }
}
