create function [dbo].[_udf_GetDataRoomFolders_WithoutFolderNumber] (
@DataRoomID int,
@Delim1 nvarchar(1) = N' ', -- delimiter between folder number and folder name
@Delim2 nvarchar(10) = N' > ' -- delimiter between subfolders
) returns @tblFolder_CTE table (
FolderID int not null primary key,
ParentFolderID int null,
IsDisabled tinyint not null,
FolderNumber nvarchar(max) not null,
FolderName nvarchar(max) not null,
FolderLevel int not null,
FolderFullName nvarchar(max) not null,
FolderPath nvarchar(max) not null,
FolderIndex int not null,
FolderSortNumber nvarchar(max) null
)
as
begin
;with tblFolder_CTE (FolderID, ParentFolderID, IsDisabled, FolderNumber, FolderName, FolderLevel, FolderFullName, FolderPath, FolderIndex)
as (
select f.FolderID, f.ParentFolderID, f.IsDisabled, cast(N'' as nvarchar(max)), cast(N'' as nvarchar(max)),
0,
cast(N'' as nvarchar(max)),
cast(N'' as nvarchar(max)),
row_number() over (partition by f.ParentFolderID order by f.FolderNumber, f.FolderName)
from tblFolder f
where f.DataRoomID = @DataRoomID and f.ParentFolderID is null

union all

select f.FolderID, f.ParentFolderID, f.IsDisabled, cast(f.FolderNumber as nvarchar(max)), cast(f.FolderName as nvarchar(max)),
f_cte.FolderLevel + 1,
cast( f.FolderName as nvarchar(max)),
cast(
case
when len(f_cte.FolderPath) > 0 then f_cte.FolderPath + @Delim2 + f.FolderName
else  f.FolderName
end as nvarchar(max)
),
row_number() over (partition by f.ParentFolderID order by f.FolderNumber, f.FolderName)
from tblFolder_CTE f_cte
join tblFolder f on f_cte.FolderID = f.ParentFolderID
)
insert @tblFolder_CTE (
FolderID, ParentFolderID, IsDisabled, FolderNumber, FolderName, FolderLevel, FolderFullName, FolderPath, FolderIndex
)
select FolderID, ParentFolderID, IsDisabled, FolderNumber, FolderName, FolderLevel, FolderFullName, FolderPath, FolderIndex
from tblFolder_CTE

declare @Level int, @DigitsCount int
set @Level = 0
set @DigitsCount = 0

update @tblFolder_CTE
set
FolderSortNumber = ''
from @tblFolder_CTE f
where f.FolderLevel = @Level

while @@rowcount > 0
begin
set @Level = @Level + 1

select @DigitsCount = floor(log10(max(FolderIndex)) + 1)
from @tblFolder_CTE f
where f.FolderLevel = @Level

update f
set
FolderSortNumber = case when len(f1.FolderSortNumber) > 0 then f1.FolderSortNumber + '.' else '' end + right(replicate('0', @DigitsCount) + cast(f.FolderIndex as varchar), @DigitsCount)
from @tblFolder_CTE f
join @tblFolder_CTE f1 on f1.FolderID = f.ParentFolderID
where f.FolderLevel = @Level
end

return
end