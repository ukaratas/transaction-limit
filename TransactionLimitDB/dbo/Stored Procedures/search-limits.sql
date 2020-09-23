CREATE PROCEDURE [dbo].[search-limits]
    @query nvarchar(400),
    @isActive bit,
    @pageIndex int,
    @pageSize int
AS
BEGIN
    SET NOCOUNT ON;
    Select *
    from [dbo].[limit-definition]
    where [path] like @query and [is-active] = @isActive
    order by [path] OFFSET @pageSize*@pageIndex ROWS FETCH NEXT @pageSize+1 ROWS ONLY
END