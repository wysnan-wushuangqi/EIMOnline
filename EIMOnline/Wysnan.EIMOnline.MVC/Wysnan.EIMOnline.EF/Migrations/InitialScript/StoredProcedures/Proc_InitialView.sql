CREATE PROCEDURE Proc_InitialView
(
	@tableName nvarchar(256)
) 
AS
BEGIN
DECLARE @msg NVARCHAR(100)
SET @msg=N'对象'+@tableName+N'不存在'

IF NULLIF(OBJECT_ID(@tableName),NULL) IS NULL
BEGIN
	RAISERROR(@msg,16,1)
	RETURN
END

SET @msg=N'对象'+@tableName+N'已经存在'

IF NULLIF((SELECT DISTINCT(EntityName) FROM zMetaFormLayout WHERE EntityName=@tableName),Null) IS NOT NULL
BEGIN
	PRINT @msg
    RETURN
END
--------------------------------------------------------【1】定义变量------------------------------------------------------------
DECLARE @TableID INT
DECLARE @fieldName varchar(100)
DECLARE @fieldType varchar(100)
DECLARE @fieldNullable INT
DECLARE @filedDescription varchar(200)
DECLARE @sortNum INT
DECLARE @isVisible BIT

SET @sortNum=1
SET @isVisible=0
SELECT @TableID=CAST(object_id(@tableName) AS INT)

--------------------------------------------------------【1】定义变量------------------------------------------------------------

--------------------------------------------------------【2】定义读取表结构的游标------------------------------------------------------------
DECLARE CReadTableName CURSOR LOCAL STATIC READ_ONLY FOR

SELECT syscolumns.name AS fieldName,
	   systypes.name AS fieldType,
CASE
	 WHEN syscolumns.isnullable is NULL  THEN 0
	 WHEN syscolumns.isnullable=1 THEN 1
	 WHEN syscolumns.isnullable=0 THEN 0
	 ELSE 0 end
     AS fieldNullable
FROM syscolumns, systypes  WHERE syscolumns.xusertype = systypes.xusertype  AND syscolumns.id =@TableID

----------------------------------------------------------定义读取表结构的游标----------------------------------------------------------------

---------------------------------------------------【3】读取游标内容,将读取结构添加到结构表中-------------------------------------------------

 OPEN CReadTableName
 FETCH NEXT FROM CReadTableName
 INTO  @fieldName,@fieldType,@fieldNullable
   
 WHILE @@FETCH_STATUS=0
 BEGIN 
  INSERT INTO dbo.zMetaFormLayout	
          ( EntityName,
            EntityField ,
            ShortLabel ,
            LongLabel ,
            IsVisible ,
            ReferenceEntity ,
            SortNum ,
            ReferenceEntityUrl ,
            Brief ,
            SystemStatus ,
            TimeStamp
          )
  VALUES  ( @tableName, -- EntityName - nvarchar(50)
            @fieldName, -- EntityField - nvarchar(50)
            @fieldName, -- ShortLabel - nvarchar(50)
            @fieldName, -- LongLabel - nvarchar(max)
            @isVisible, -- IsVisible - bit 
            N'' , -- ReferenceEntity - nvarchar(100)
            @sortNum , -- SortNum - int
            '', -- ReferenceEntityUrl - nvarchar(200)
            @fieldName+N' Descriptions ', -- Brief - nvarchar(100)
            0 , -- SystemStatus - tinyint
            NULL  -- TimeStamp - timestamp
          ) 

        FETCH NEXT FROM CReadTableName
		INTO  @fieldName,@fieldType,@fieldNullable	
		
	    SET @sortNum=@sortNum+1

		IF(UPPER(@fieldName)='ID' OR UPPER(@fieldName)='TIMESTAMP' OR UPPER(@fieldName)='SYSTEMSTATUS')
			SET @isVisible=0
		ELSE SET @isVisible=1
 END 
CLOSE CReadTableName
DEALLOCATE CReadTableName
END
