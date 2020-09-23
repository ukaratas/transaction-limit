CREATE PROCEDURE [dbo].[insert-update-limit-definition]
    @amountRemainingLimit money,
    @path nvarchar(400),
    @currencyCode char(3),
    @transactionMin money,
    @transactionMax money,
    @amountLimit money,
    @timerLimit int,
    @duration nvarchar(20),
    @isActive bit,
    @renewal tinyint,
    @renewedAt datetime2(7),
    @createdAt datetime2(7),
    @availability nvarchar(max),
    @amountUtilizedLimit money,
    @timerRemainingLimit int,
    @maxAmountLimit money,
    @maxAmountLimitCurrencyCode char(3),
    @defaultAmountLimit money,
    @defaultTimerLimit int,
    @maxTimerLimit int,
    @alsoLook nvarchar(400)
AS
BEGIN
    IF EXISTS (SELECT 1
    FROM [dbo].[limit-definition]
    WHERE [path] = @path AND [duration] = @duration) 
    BEGIN
        UPDATE [dbo].[limit-definition]
                SET
                    [amount-remaining-limit] = @amountRemainingLimit,
                    [timer-remaining-limit] = @timerRemainingLimit,
                    [currency-code] = @currencyCode,
                    [transaction-min-limit] = @transactionMin,
                    [transaction-max-limit] = @transactionMax,
                    [amount-limit] = @amountLimit,
                    [timer-limit] = @timerLimit,
                    [is-active] = @isActive,
                    [renewal] = @renewal,
                    [renewed-at] = @renewedAt,
                    [created-at] = @createdAt,
                    [availability] = @availability,
					[amount-utilized-limit] = @amountUtilizedLimit,
                    [max-amount-limit] = @maxAmountLimit,
                    [max-amount-limit-currency-code] = @maxAmountLimitCurrencyCode,
                    [default-amount-limit] = @defaultAmountLimit,
                    [default-timer-limit] = @defaultTimerLimit,
                    [max-timer-limit] = @maxTimerLimit,
                    [also-look] = @alsoLook
                WHERE [path]=@path and [duration]=@duration
    END
    ELSE
    BEGIN
        INSERT INTO [dbo].[limit-definition]
            (
            [amount-remaining-limit],
            [amount-utilized-limit],
            [timer-remaining-limit],
            [timer-utilized-limit],
            [path],
            [currency-code],
            [transaction-min-limit],
            [transaction-max-limit],
            [amount-limit],
            [timer-limit],
            [duration],
            [is-active],
            [renewal],
            [renewed-at],
            [created-at],
            [availability],
            [max-amount-limit],
            [max-amount-limit-currency-code],
            [default-amount-limit],
            [default-timer-limit],
            [max-timer-limit],
            [also-look]
            )
        VALUES
            (
                @amountLimit,
                0,
                @timerLimit,
                0,
                @path,
                @currencyCode,
                @transactionMin,
                @transactionMax,
                @amountLimit,
                @timerLimit,
                @duration,
                @isActive,
                @renewal,
                @renewedAt,
                @createdAt,
                @availability,
                @maxAmountLimit,
                @maxAmountLimitCurrencyCode,
                @defaultAmountLimit,
                @defaultTimerLimit,
                @maxTimerLimit,
                @alsoLook
            )
    END
END