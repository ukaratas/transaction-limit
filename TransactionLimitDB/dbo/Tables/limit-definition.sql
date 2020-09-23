CREATE TABLE [dbo].[limit-definition]
(
    [path] NVARCHAR (400) NOT NULL,
    [default-amount-limit] MONEY NULL,
    [default-timer-limit] INT NULL,
    [max-amount-limit] MONEY NULL,
    [max-amount-limit-currency-code] char (3) NULL,
    [max-timer-limit] INT NULL,
    [transaction-min-limit] MONEY NULL,
    [transaction-max-limit] MONEY NULL,
    [currency-code] CHAR (3) NULL,
    [amount-limit] MONEY NULL,
    [amount-remaining-limit] MONEY NULL,
    [amount-utilized-limit] MONEY NULL,
    [timer-limit] INT NULL,
    [timer-remaining-limit] INT NULL,
    [timer-utilized-limit] INT NULL,
    [duration] NVARCHAR (20) NOT NULL,
    [is-active] BIT NULL,
    [renewal] TINYINT NULL,
    [renewed-at] DATETIME2 (7) NULL,
    [created-at] DATETIME2 (7) NULL,
    [availability] NVARCHAR (MAX) NULL,
    [also-look] NVARCHAR (400) NULL,
    CONSTRAINT [limit-definition_primaryKey] PRIMARY KEY NONCLUSTERED HASH ([path],[duration]) WITH (BUCKET_COUNT = 64)
)
WITH (MEMORY_OPTIMIZED = ON);

