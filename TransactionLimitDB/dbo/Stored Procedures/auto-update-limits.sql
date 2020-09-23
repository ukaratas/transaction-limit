CREATE PROCEDURE [dbo].[auto-update-limits]
	@path nvarchar(400),
	@duration nvarchar(20),
	@renewedAt DateTime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	UPDATE  
		[limit-definition]
	SET 
      [amount-remaining-limit] =  [amount-limit],
      [amount-utilized-limit] = 0,
      [timer-remaining-limit] = [timer-limit], 
      [timer-utilized-limit] = 0,
	  [renewed-at] = @renewedAt
  FROM [limit-definition]
  WHERE [path] = @path AND [is-active] = 1 AND [duration] = @duration
END