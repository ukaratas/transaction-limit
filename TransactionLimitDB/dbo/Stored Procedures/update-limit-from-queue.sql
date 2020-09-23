CREATE PROCEDURE [dbo].[update-limit-from-queue]
AS
BEGIN
	DECLARE @handle UNIQUEIDENTIFIER;
	DECLARE @message NVARCHAR(MAX);
	DECLARE @message_type INT;
	SET NOCOUNT ON;
	WHILE(1=1)
	BEGIN
		WAITFOR (
		RECEIVE top(1) 
			@message = CAST(message_body AS NVARCHAR(MAX)),      
			@handle = conversation_handle,
			@message_type=message_type_id
			FROM [update-limit-queue]
		), TIMEOUT 1000

		if (@@ROWCOUNT = 0)
		BEGIN
			ROLLBACK TRANSACTION
			BREAK
		END

		IF (@message_type = 14)
		BEGIN
			DECLARE @v_path nvarchar(400) = JSON_VALUE ( @message,  '$."path"' );
			DECLARE @v_duration nvarchar(20) = JSON_VALUE ( @message,  '$."duration"' );
			DECLARE @v_amount money = JSON_VALUE ( @message, '$."amount"' );
			DECLARE @v_type tinyint = JSON_VALUE (@message,'$."type"');

			DECLARE	@return_value int
			EXEC	@return_value = [dbo].[update-limit]
					@path = @v_path,
					@duration=@v_duration,
					@amount = @v_amount,
					@type = @v_type
		END
	END

END