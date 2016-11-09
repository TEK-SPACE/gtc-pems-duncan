USE [PEMS_RBAC_DATABASE_NAME]
GO


DECLARE @CustomerId int
DECLARE @CustomerSettingTypeId int
DECLARE @DefaultPassword varchar(255)

SELECT @CustomerId = [CustomerId] from [dbo].[CustomerProfile] where [CustomerTypeId] = 0
SELECT @CustomerSettingTypeId = [CustomerSettingTypeId] , @DefaultPassword = [DefaultValue]
		FROM [dbo].[CustomerSettingType] where CustomerSettingTypeName = 'DefaultPassword'


IF NOT EXISTS (select * from [dbo].[CustomerSettings] where [CustomerId] = @CustomerId 
	AND [CustomerSettingTypeId] = @CustomerSettingTypeId)
BEGIN

	-- Uncomment and set following to pick your own default password
	-- SET @DefaultPassword = 'NEW_DEFAULT_PASSWORD'

	INSERT INTO [dbo].[CustomerSettings] ([CustomerId],[CustomerSettingTypeId],[SettingValue])
     VALUES (@CustomerId, @CustomerSettingTypeId, @DefaultPassword)

	PRINT 'Default password set to ' + @DefaultPassword

END

