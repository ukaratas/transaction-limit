CREATE SERVICE [update-limit-request-queue-service]
    AUTHORIZATION [dbo]
    ON QUEUE [dbo].[update-limit-request-queue]
    ([DEFAULT]);

