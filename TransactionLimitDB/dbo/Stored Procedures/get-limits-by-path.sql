CREATE PROCEDURE [dbo].[get-limits-by-path]
  @paths [path-array-type] READONLY
AS
BEGIN
  SELECT
    [path],
    [amount-limit],
    [amount-remaining-limit],
    [amount-utilized-limit],
    [transaction-min-limit],
    [transaction-max-limit],
    [timer-limit],
    [timer-remaining-limit],
    [timer-utilized-limit],
    [availability],
    [duration],
    [renewal],
    [renewed-at],
    [created-at],
    [currency-code],
    [max-amount-limit-currency-code],
    [max-amount-limit],
    [default-amount-limit],
    [default-timer-limit],
    [max-timer-limit],
    [is-active],
    [also-look]
  FROM [dbo].[limit-definition]  WITH (SNAPSHOT)
  WHERE [path] IN  (SELECT paths.[path]
  FROM @paths AS paths)
END