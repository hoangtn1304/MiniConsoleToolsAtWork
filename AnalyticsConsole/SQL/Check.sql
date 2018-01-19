IF EXISTS (
    SELECT * FROM sysobjects WHERE id = object_id(N'_udf_GetDataRoomFolders_WithoutFolderNumber') 
    AND xtype IN (N'FN', N'IF', N'TF')
)
    DROP FUNCTION _udf_GetDataRoomFolders_WithoutFolderNumber