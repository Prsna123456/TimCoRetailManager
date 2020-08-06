CREATE PROCEDURE [dbo].[spInventory_GetAll]
AS
BEGIN

	set nocount on;

	SELECT [ProductId], [Quantity], [PurchasePrice], [PurchaseDate]
	from dbo.Inventory;
END
