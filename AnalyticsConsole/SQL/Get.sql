declare @FullFolderPathResult table (DataRoomID int, FolderID int, FolderPath varchar(4000))

declare @DataRoomsList table(ID int not null)
insert into @DataRoomsList 
	select DataRoomID 
	from tblDataRoom dr, tblGicsSubIndustry gs
	where dr.InsertDate > '2012-01-01' 
		and dr.IsDemo = 0 
		and dr.GicsSubIndustryCode = gs.GicsSubIndustryCode
		and gs.GicsIndustryGroupCode = @industryGroupCode

DECLARE @CurrentDataRoomID int
DECLARE db_cursor CURSOR FOR 
SELECT ID from @DataRoomsList

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @CurrentDataRoomID   

WHILE @@FETCH_STATUS = 0   
BEGIN   
      insert into @FullFolderPathResult select @CurrentDataRoomID, FolderID, FolderPath from [_udf_GetDataRoomFolders_WithoutFolderNumber](@CurrentDataRoomID,' ',' \ ')
       FETCH NEXT FROM db_cursor INTO @CurrentDataRoomID   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor

select * from @FullFolderPathResult