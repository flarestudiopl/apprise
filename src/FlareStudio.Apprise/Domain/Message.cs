using System;

namespace FlareStudio.Apprise.Domain
{
    internal class Message
    {
        public int MessageId { get; set; }
        public string TemplateKey { get; set; }
        public string Model { get; set; }
        public string Recipients { get; set; }
        public byte AttemptsPerformed { get; set; }
        public MessageState MessageState { get; set; }
        public string LastAttemptError { get; set; }
        public DateTime? LastAttemptTimestampUtc { get; set; }
    }
}
