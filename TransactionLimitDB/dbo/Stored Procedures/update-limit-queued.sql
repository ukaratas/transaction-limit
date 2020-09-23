CREATE PROCEDURE [dbo].[update-limit-queued]
	@path nvarchar(400),
	@duration nvarchar(20),
	@amount money,
	@type tinyint
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @handle UNIQUEIDENTIFIER;
	DECLARE @request nvarchar(1000);
	SET @request  = '{ "path":"'+ @path + '", "duration":"' + @duration + '", "amount":' + CAST(CAST(@amount AS numeric(22,4)) AS VARCHAR) + ', "type":"'+ CAST(@type AS VARCHAR) +'"}'

	BEGIN DIALOG @handle  
	FROM SERVICE [update-limit-request-queue-service]
	TO SERVICE 'update-limit-queue-service'
	WITH
	ENCRYPTION = OFF;

	SEND ON CONVERSATION @handle( @request);

	SELECT @handle

END