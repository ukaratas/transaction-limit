CREATE SERVICE [update-limit-queue-service]
    AUTHORIZATION [dbo]
    ON QUEUE [dbo].[update-limit-queue]
    ([DEFAULT]);

