CREATE PROCEDURE [dbo].[patch-limit]
    @path nvarchar(400),
    @duration nvarchar(20),
    @amountUtilizedLimit money,
    @timerUtilizedLimit int
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @amountremaininglimit money;
    DECLARE @timerremaininglimit INT;

    IF @amountUtilizedLimit=-1
    BEGIN
        SELECT @amountUtilizedLimit=[amount-utilized-limit]
        FROM [limit-definition]
        WHERE [path]=@path AND [duration] = @duration
    END

    IF @timerUtilizedLimit=-1
    BEGIN
        SELECT @timerUtilizedLimit=[timer-utilized-limit]
        FROM [limit-definition]
        WHERE [path]=@path AND [duration] = @duration
    END

    SELECT @amountremaininglimit = [amount-limit]-@amountUtilizedLimit, @timerremaininglimit = [timer-limit]-@timerUtilizedLimit
    FROM [limit-definition]
    WHERE [path]=@path AND [duration] = @duration

    UPDATE [limit-definition]
	    SET 
      [amount-remaining-limit] =  @amountremaininglimit,
      [amount-utilized-limit] = @amountUtilizedLimit,
      [timer-remaining-limit] = @timerremaininglimit,
      [timer-utilized-limit] = @timerUtilizedLimit
      FROM [limit-definition] WHERE [path] = @path AND [duration] = @duration

END