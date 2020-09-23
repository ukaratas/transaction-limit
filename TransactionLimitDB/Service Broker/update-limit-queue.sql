CREATE QUEUE [dbo].[update-limit-queue]
    WITH ACTIVATION (STATUS = ON, PROCEDURE_NAME = [dbo].[update-limit-from-queue], MAX_QUEUE_READERS = 1, EXECUTE AS OWNER);

