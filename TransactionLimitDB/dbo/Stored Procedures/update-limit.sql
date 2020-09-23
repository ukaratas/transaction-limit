CREATE PROCEDURE [dbo].[update-limit]
  @path nvarchar(400),
  @duration nvarchar(20),
  @amount money,
  @type tinyint
AS
BEGIN
  SET NOCOUNT ON;

  DECLARE @amountRemainingLimit money;
  DECLARE @amountUtilizedLimit money;
  DECLARE @amountLimit money;
  DECLARE @timerRemainingLimit INT;
  DECLARE @timerUtilizedLimit INT;
  DECLARE @timerLimit INT;

  SELECT @amountRemainingLimit = [amount-remaining-limit], @amountUtilizedLimit=[amount-utilized-limit],
    @timerRemainingLimit = [timer-remaining-limit], @timerUtilizedLimit = [timer-utilized-limit],
    @timerLimit=[timer-limit], @amountLimit = [amount-limit]
  FROM [limit-definition]
  where [path]=@path AND [duration] = @duration AND [is-active] = 1

  IF @type=1
	BEGIN
    IF (@amountRemainingLimit >= @amount OR @amountLimit = -1) AND (@timerRemainingLimit > 0 OR @timerLimit = -1)
    BEGIN
      IF(@amountLimit = -1 AND @timerLimit = -1)
      BEGIN
        UPDATE [limit-definition]
	    SET 
      [amount-utilized-limit] = [amount-utilized-limit] + @amount,
      [timer-utilized-limit] = [timer-utilized-limit] + 1
      FROM [limit-definition] WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
        RETURN 0
      END
      ELSE IF(@timerLimit = -1)
      BEGIN
        UPDATE [limit-definition]
	    SET 
      [amount-remaining-limit] =  [amount-remaining-limit] - @amount,
      [amount-utilized-limit] = [amount-utilized-limit] + @amount,
      [timer-utilized-limit] = [timer-utilized-limit] + 1
      FROM [limit-definition] WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
      END
      ELSE IF(@amountLimit = -1)
      BEGIN
        UPDATE [limit-definition]
	    SET 
      [amount-utilized-limit] = [amount-utilized-limit] + @amount,
      [timer-remaining-limit] = [timer-remaining-limit] - 1, 
      [timer-utilized-limit] = [timer-utilized-limit] + 1
      FROM [limit-definition] WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
      END
      ELSE
      BEGIN
        UPDATE [limit-definition]
	    SET 
      [amount-remaining-limit] =  [amount-remaining-limit] - @amount,
      [amount-utilized-limit] = [amount-utilized-limit] + @amount,
      [timer-remaining-limit] = [timer-remaining-limit] - 1, 
      [timer-utilized-limit] = [timer-utilized-limit] + 1
      FROM [limit-definition] WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
      END
      RETURN 0
    END
    ELSE 
    BEGIN
      IF @amountRemainingLimit < @amount
      BEGIN
        RETURN 1
      END
      ELSE IF @timerRemainingLimit <= 0
      BEGIN
        RETURN 2
      END
    End
  END
ELSE
IF @type=2
  BEGIN
    IF @amountUtilizedLimit >= @amount AND @timerUtilizedLimit > 0
    BEGIN
      IF(@amountLimit = -1 AND @timerLimit = -1)
    BEGIN
        UPDATE [limit-definition]
	    SET 
      [amount-utilized-limit] = [amount-utilized-limit] - @amount,
      [timer-utilized-limit] = [timer-utilized-limit] - 1
      FROM [limit-definition] WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
      END
    ELSE IF (@amountLimit = -1)
    BEGIN
        UPDATE [limit-definition]
	    SET 
      [amount-utilized-limit] = [amount-utilized-limit] - @amount,
      [timer-remaining-limit] = [timer-remaining-limit] + 1, 
      [timer-utilized-limit] = [timer-utilized-limit] - 1
      FROM [limit-definition] WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
      END
    ELSE IF (@timerLimit = -1)
    BEGIN
        UPDATE [limit-definition]
	    SET 
      [amount-remaining-limit] =  [amount-remaining-limit] + @amount,
      [amount-utilized-limit] = [amount-utilized-limit] - @amount,
      [timer-utilized-limit] = [timer-utilized-limit] - 1
      FROM [limit-definition] WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
      END
    ELSE
    BEGIN
        UPDATE [limit-definition]
	    SET 
      [amount-remaining-limit] =  [amount-remaining-limit] + @amount,
      [amount-utilized-limit] = [amount-utilized-limit] - @amount,
      [timer-remaining-limit] = [timer-remaining-limit] + 1, 
      [timer-utilized-limit] = [timer-utilized-limit] - 1
      FROM [limit-definition] WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
      END
      RETURN 0
    END
    ELSE 
    BEGIN
    IF @amountUtilizedLimit < @amount
      BEGIN
      RETURN 1
    END
      ELSE IF @timerUtilizedLimit <= 0
      BEGIN
      RETURN 2
    END
  END
END
END