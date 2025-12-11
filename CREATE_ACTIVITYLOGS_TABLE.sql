-- Create ActivityLogs table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ActivityLogs]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[ActivityLogs](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [UserId] [int] NULL,
        [Username] [nvarchar](100) NOT NULL,
        [Action] [nvarchar](100) NOT NULL,
        [EntityType] [nvarchar](100) NOT NULL,
        [EntityId] [int] NULL,
        [Description] [nvarchar](500) NULL,
        [IpAddress] [nvarchar](50) NULL,
        [CreatedAt] [datetime] NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_ActivityLogs] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ActivityLogs_Users] FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE SET NULL
    )
    
    CREATE INDEX [IX_ActivityLogs_UserId] ON [dbo].[ActivityLogs]([UserId])
    CREATE INDEX [IX_ActivityLogs_CreatedAt] ON [dbo].[ActivityLogs]([CreatedAt] DESC)
    CREATE INDEX [IX_ActivityLogs_Action] ON [dbo].[ActivityLogs]([Action])
END
GO










