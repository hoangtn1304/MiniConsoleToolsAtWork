select top 100 gig.GicsIndustryGroupName, f.FolderName, count(*) as 'DataRoom Appear'
from tblDataRoom dr left join tblFolder f on dr.DataRoomID = f.DataRoomID
					left join tblGicsSubIndustry gsi on dr.GicsSubIndustryCode = gsi.GicsSubIndustryCode
					left join tblGicsIndustryGroup gig on gsi.GicsIndustryGroupCode = gig.GicsIndustryGroupCode
where dr.GicsSubIndustryCode IS NOT NULL
and gig.GicsIndustryGroupCode = @industryGroupCode
group by gig.GicsIndustryGroupName, f.FolderName
order by gig.GicsIndustryGroupName ASC, [DataRoom Appear] DESC