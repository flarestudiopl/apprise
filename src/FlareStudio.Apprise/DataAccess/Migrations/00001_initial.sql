CREATE TABLE [Message] (
    [MessageId]                INT IDENTITY    NOT NULL,
    [TemplateKey]              NVARCHAR(250)   NOT NULL,
    [Model]                    NVARCHAR(MAX)   NULL,
    [Recipients]               NVARCHAR(4000)  NOT NULL,
    [AttemptsPerformed]        TINYINT         NOT NULL DEFAULT(0),
    [MessageState]             TINYINT         NOT NULL DEFAULT(0),
    [LastAttemptError]         NVARCHAR(4000)  NULL,
    [LastAttemptTimestampUtc]  DATETIME2       NULL,
    CONSTRAINT PK_Message_MessageId PRIMARY KEY CLUSTERED (MessageId)
)

GO
CREATE NONCLUSTERED INDEX IX_Message_MessageState
    ON [Message] (MessageState);
