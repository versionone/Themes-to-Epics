/*	
 *	Create Epics representing Themes, and assign top-level epics and stories to the new Epics.
 *	
 *	Set @projectID to the ID of the project to which to limit changes.  Using 0 will affect all projects.
 *	
 *	Insert records into @customFields to describe custom fields to copy from Themes to Epics.
 *	
 *	Set @saveChanges to 1 in order to commit changes.  Otherwise, all changes are rolled back.
 *	
 *	NOTE:  This script defaults to rolling back changes.
 *		To commit changes, set @saveChanges = 1.
 */

alter table dbo.IDSource add ThemeID int null, IsNew bit null, Name int null

GO

declare @projectID int; select @projectID = 0

declare @customFields table(FromThemeCustomField varchar(200) collate Latin1_General_BIN, ToEpicCustomField varchar(200) collate Latin1_General_BIN)
insert @customFields values('Theme.Custom_Is_MMF2', 'PrimaryWorkitem.Custom_Is_MMF')

declare @saveChanges bit; --set @saveChanges = 1


-- Ensure the correct database version
declare @supportedVersions varchar(1000); select @supportedVersions='11.2.*'
BEGIN
	declare @sep char(2); select @sep=', '
	if not exists(select *
		from dbo.SystemConfig
		join (
		select SUBSTRING(@supportedVersions, C.Value+1, CHARINDEX(@sep, @supportedVersions+@sep, C.Value+1)-C.Value-1) as Value
		from dbo.Counter C
		where C.Value < DataLength(@supportedVersions) and SUBSTRING(@sep+@supportedVersions, C.Value+1, DataLength(@sep)) = @sep
		) Version on SystemConfig.Value like REPLACE(Version.Value, '*', '%') and SystemConfig.Name = 'Version'
	) begin
			raiserror('Only supported on version(s) %s',16,1, @supportedVersions)
			goto DONE
	end
END

declare @error int, @rowcount varchar(20)
set nocount on; begin tran; save tran TX


declare @projectName nvarchar(4000)
select @projectName=Name.Value from BaseAsset_Now Project join String Name on Name.ID=Name where Project.ID=@projectID
if @@ROWCOUNT=0 begin
	raiserror('No such project Scope:%d',16,1, @projectID)
	goto ERR
end else begin
	print 'Project: ' + @projectName
end

declare @assetStringUpdates table(AssetID int, StringDefinition varchar(200) collate Latin1_General_BIN, OldString int, NewString int, LongStringDefinition varchar(200) collate Latin1_General_BIN, OldLongString int, NewLongString int)

declare @auditid int, @changereasonid int, @changecommentid int
exec dbo._SaveString N'Script', @changereasonid output
exec dbo._SaveString N'Themes-to-Epics', @changecommentid output
insert dbo.Audit(ChangeDateUTC, ChangeReason, ChangeComment) values(GETUTCDATE(), @changereasonid, @changecommentid)
select @auditid=SCOPE_IDENTITY(), @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

-- find Themes that already were converted
update dbo.IDSource 
set ThemeID=Theme_Now.ID, IsNew=0, Name=BaseAsset_Now.Name
from dbo.BaseAsset_Now 
join dbo.Workitem_Now on Workitem_Now.ID=BaseAsset_Now.ID and AssetState<192
join dbo.Theme_Now on Theme_Now.ID=Workitem_Now.ID
join dbo.ScopeParentHierarchy on AncestorID=@projectID and DescendantID=Workitem_Now.ScopeID and ScopeParentHierarchy.AuditEnd is null
join dbo.CustomRelation on ForeignID=Theme_Now.ID and Definition='Story.GeneratedFromTheme' and CustomRelation.AuditEnd is null
where IDSource.ID=PrimaryID

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print 'Refreshing ' + @rowcount + ' Themes'

-- find Themes that have not been converted
insert dbo.IDSource(ThemeID, IsNew, Name)
select Theme_Now.ID, 1, BaseAsset_Now.Name
from dbo.BaseAsset_Now 
join dbo.Workitem_Now on Workitem_Now.ID=BaseAsset_Now.ID and AssetState<192
join dbo.Theme_Now on Theme_Now.ID=Workitem_Now.ID
join dbo.ScopeParentHierarchy on AncestorID=@projectID and DescendantID=Workitem_Now.ScopeID and ScopeParentHierarchy.AuditEnd is null
where not exists (select PrimaryID from CustomRelation where ForeignID=Theme_Now.ID and Definition='Story.GeneratedFromTheme' and AuditEnd is null)

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print 'Converting ' + @rowcount + ' Themes'


-- calculate new names for new epics
declare @epicID int, @themeID int, @nameID int, @name nvarchar(4000)
declare C cursor local fast_forward for
	select IDSource.ID, IDSource.Name, Name.Value 
	from dbo.IDSource 
	join dbo.String Name on Name.ID=Name
open C
while 1=1 begin
	fetch next from C into @epicID, @nameID, @name
	if @@FETCH_STATUS<>0 break

	declare @newName nvarchar(4000), @newNameID int
	select @newName=N'Feature - ' + @name
	exec dbo._SaveString @newName, @newNameID output

	update dbo.IDSource
	set Name=@newNameID
	where ID=@epicID and Name=@nameID
end
close C; deallocate C


-- reserve numbers for the new epics
declare @number int
select @number=LastNumber from dbo.NumberSource where Code='Story'

update dbo.NumberSource
set LastNumber=LastNumber + (select count(*) from dbo.IDSource where IsNew=1)
where Code='Story'

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR


-- insert or update epic's BaseAsset
--Name
--Description
update dbo.BaseAsset_Now
set AuditBegin=@auditid, Name=Epic.Name, Description=Theme.Description, Substate=Theme.AssetState, SecurityScopeID=Theme.SecurityScopeID
output inserted.ID ID, 'BaseAsset.Name', deleted.Name, inserted.Name, 'BaseAsset.Description', deleted.Description, inserted.Description into @assetStringUpdates
from dbo.BaseAsset_Now
join dbo.IDSource Epic on Epic.ID=BaseAsset_Now.ID and IsNew=0
join dbo.BaseAsset_Now Theme on Theme.ID=Epic.ThemeID

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' updated Epics (BaseAsset)'

insert dbo.BaseAsset_Now(ID, AssetType, AuditBegin, Name, Description, AssetState, Substate, SecurityScopeID)
output inserted.ID ID, 'BaseAsset.Name', null, inserted.Name, 'BaseAsset.Description', null, inserted.Description into @assetStringUpdates
select Epic.ID, 'Story', @auditid, Epic.Name, Theme.Description, 208, Theme.AssetState, Theme.SecurityScopeID
from dbo.BaseAsset_Now Theme
join dbo.IDSource Epic on Epic.ThemeID=Theme.ID
where IsNew=1

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' new Epics (BaseAsset)'

-- insert or update epic's Workitem
--Scope
update dbo.Workitem_Now
set AuditBegin=@auditid, ScopeID=Theme.ScopeID
from dbo.Workitem_Now
join dbo.IDSource Epic on Epic.ID=Workitem_Now.ID and IsNew=0
join dbo.Workitem_Now Theme on Theme.ID=Epic.ThemeID

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' updated Epics (Workitem)'

insert dbo.Workitem_Now(ID, AssetType, AuditBegin, ScopeID, Number)
select Epic.ID, 'Story', @auditid, Theme.ScopeID, @number + ROW_NUMBER() over (order by Epic.ID)
from dbo.Workitem_Now Theme
join dbo.IDSource Epic on Epic.ThemeID=Theme.ID
where IsNew=1

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' new Epics (Workitem)'

-- insert or update epic's PrimaryWorkitem
--Priority
--Estimate
update dbo.PrimaryWorkitem_Now
set AuditBegin=@auditid, Estimate=Theme.Estimate, PriorityID=Theme.PriorityID
from dbo.PrimaryWorkitem_Now
join dbo.IDSource Epic on Epic.ID=PrimaryWorkitem_Now.ID and IsNew=0
join dbo.Theme_Now Theme on Theme.ID=Epic.ThemeID

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' updated Epics (PrimaryWorkitem)'

insert dbo.PrimaryWorkitem_Now(ID, AssetType, AuditBegin, Estimate, PriorityID)
select Epic.ID, 'Story', @auditid, Theme.Estimate, Theme.PriorityID
from dbo.Theme_Now Theme
join dbo.IDSource Epic on Epic.ThemeID=Theme.ID
where IsNew=1

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' new Epics (PrimaryWorkitem)'

-- insert or update epic's Story
--Risk
update dbo.Story_Now
set AuditBegin=@auditid, RiskID=Theme.RiskID
from dbo.Story_Now
join dbo.IDSource Epic on Epic.ID=Story_Now.ID and IsNew=0
join dbo.Theme_Now Theme on Theme.ID=Epic.ThemeID

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' updated Epics (Story)'

insert dbo.Story_Now(ID, AssetType, AuditBegin, RiskID)
select Epic.ID, 'Story', @auditid, Theme.RiskID
from dbo.Theme_Now Theme
join dbo.IDSource Epic on Epic.ThemeID=Theme.ID
where IsNew=1

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' new Epics (Story)'

-- copy Theme Owners to Epic Owners
--Owners
update dbo.WorkitemOwners set AuditEnd=@auditid
from dbo.IDSource Epic
where WorkitemID=Epic.ID and IsNew is not null and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.WorkitemOwners(WorkitemID, MemberID, AuditBegin)
select Epic.ID, ThemeOwners.MemberID, @auditid
from dbo.IDSource Epic
join dbo.WorkitemOwners ThemeOwners on ThemeOwners.WorkitemID=ThemeID and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' Owners'

-- copy Theme Goals to Epic Goals
--Goals
update dbo.WorkitemGoals set AuditEnd=@auditid
from dbo.IDSource Epic
where WorkitemID=Epic.ID and IsNew is not null and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.WorkitemGoals(WorkitemID, GoalID, AuditBegin)
select Epic.ID, ThemeGoals.GoalID, @auditid
from dbo.IDSource Epic
join dbo.WorkitemGoals ThemeGoals on ThemeGoals.WorkitemID=ThemeID and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' Goals'

--Reference
declare C cursor local fast_forward for
	select ID, ThemeID from dbo.IDSource where IsNew=1
open C
while 1=1 begin
	fetch next from C into @epicID, @themeID
	if @@FETCH_STATUS<>0 break

	declare @reference nvarchar(4000), @stringID int
	select @reference=N'generated-from:Theme:' + cast(@themeID as nvarchar(10))
	exec dbo._SaveString @reference, @stringID output

	insert dbo.CustomText(ID, Definition, AuditBegin, Value)
	values (@epicID, 'Workitem.Reference', @auditid, @stringID)

	insert @assetStringUpdates values (@epicID, 'Workitem.Reference', null, @stringID, null, null, null)
end
close C; deallocate C

--CustomFields
update dbo.CustomBoolean set AuditEnd=@auditid
from dbo.IDSource Epic
where CustomBoolean.ID=Epic.ID and IsNew is not null and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.CustomBoolean(ID, Definition, AuditBegin, Value)
select Epic.ID, ToEpicCustomField, @auditid, ThemeCustom.Value
from dbo.IDSource Epic
join dbo.CustomBoolean ThemeCustom on ThemeCustom.ID=Epic.ThemeID and ThemeCustom.AuditEnd is null
join @customFields CustomFields on CustomFields.FromThemeCustomField=ThemeCustom.Definition

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' Custom Boolean values'

update dbo.CustomDate set AuditEnd=@auditid
from dbo.IDSource Epic
where CustomDate.ID=Epic.ID and IsNew is not null and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.CustomDate(ID, Definition, AuditBegin, Value)
select Epic.ID, ToEpicCustomField, @auditid, ThemeCustom.Value
from dbo.IDSource Epic
join dbo.CustomDate ThemeCustom on ThemeCustom.ID=Epic.ThemeID and ThemeCustom.AuditEnd is null
join @customFields CustomFields on CustomFields.FromThemeCustomField=ThemeCustom.Definition

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' Custom Date values'

update dbo.CustomLongText set AuditEnd=@auditid
output deleted.ID, null, null, null, deleted.Definition, deleted.Value, null into @assetStringUpdates
from dbo.IDSource Epic
where CustomLongText.ID=Epic.ID and IsNew is not null and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.CustomLongText(ID, Definition, AuditBegin, Value)
output inserted.ID, null, null, null, inserted.Definition, null, inserted.Value into @assetStringUpdates
select Epic.ID, ToEpicCustomField, @auditid, ThemeCustom.Value
from dbo.IDSource Epic
join dbo.CustomLongText ThemeCustom on ThemeCustom.ID=Epic.ThemeID and ThemeCustom.AuditEnd is null
join @customFields CustomFields on CustomFields.FromThemeCustomField=ThemeCustom.Definition

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' Custom LongText values'

update dbo.CustomNumeric set AuditEnd=@auditid
from dbo.IDSource Epic
where CustomNumeric.ID=Epic.ID and IsNew is not null and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.CustomNumeric(ID, Definition, AuditBegin, Value)
select Epic.ID, ToEpicCustomField, @auditid, ThemeCustom.Value
from dbo.IDSource Epic
join dbo.CustomNumeric ThemeCustom on ThemeCustom.ID=Epic.ThemeID and ThemeCustom.AuditEnd is null
join @customFields CustomFields on CustomFields.FromThemeCustomField=ThemeCustom.Definition

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' Custom Numeric values'

update dbo.CustomText set AuditEnd=@auditid
output deleted.ID, deleted.Definition, deleted.Value, null, null, null, null into @assetStringUpdates
from dbo.IDSource Epic
where CustomText.ID=Epic.ID and IsNew is not null and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.CustomText(ID, Definition, AuditBegin, Value)
output inserted.ID, inserted.Definition, null, inserted.Value, null, null, null into @assetStringUpdates
select Epic.ID, ToEpicCustomField, @auditid, ThemeCustom.Value
from dbo.IDSource Epic
join dbo.CustomText ThemeCustom on ThemeCustom.ID=Epic.ThemeID and ThemeCustom.AuditEnd is null
join @customFields CustomFields on CustomFields.FromThemeCustomField=ThemeCustom.Definition

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' Custom Text values'

update dbo.CustomRelation set AuditEnd=@auditid
from dbo.IDSource Epic
where CustomRelation.PrimaryID=Epic.ID and IsNew is not null and AuditEnd is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.CustomRelation(PrimaryID, Definition, AuditBegin, ForeignID)
select Epic.ID, ToEpicCustomField, @auditid, ThemeCustom.ForeignID
from dbo.IDSource Epic
join dbo.CustomRelation ThemeCustom on ThemeCustom.PrimaryID=Epic.ThemeID and ThemeCustom.AuditEnd is null
join @customFields CustomFields on CustomFields.FromThemeCustomField=ThemeCustom.Definition

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' Custom Relation values'

insert dbo.CustomRelation(Definition, PrimaryID, ForeignID, AuditBegin)
select 'Story.GeneratedFromTheme', Epic.ID, Epic.ThemeID, @auditid
from dbo.IDSource Epic
where IsNew=1

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR


-- Tree generated epics
update dbo.Workitem_Now set AuditBegin=@auditid, SuperID=Super.ID
from dbo.Workitem_Now 
join dbo.IDSource Sub on Sub.ID=Workitem_Now.ID
join dbo.Workitem_Now Theme on Theme.ID=Sub.ThemeID
join dbo.IDSource Super on Super.ThemeID=Theme.ParentID

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' epics parented'


-- Assign top-level themed epics and stories to generated Epics
update dbo.Workitem_Now set AuditBegin=@auditid, SuperID=Super.ID
from dbo.Workitem_Now
join dbo.BaseAsset_Now on BaseAsset_Now.ID=Workitem_Now.ID and (AssetState<192 or (AssetState=208 and SubState<192))
join dbo.ScopeParentHierarchy on AncestorID=@projectID and DescendantID=Workitem_Now.ScopeID and ScopeParentHierarchy.AuditEnd is null
join dbo.IDSource Super on Super.ThemeID=Workitem_Now.ParentID
where Workitem_Now.AssetType='Story' and Workitem_Now.SuperID is null 
	and Workitem_Now.ID not in (select ID from dbo.IDSource where ThemeID is not null)

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' stories and epics assigned'


-- track strings
delete dbo.AssetString
from @assetStringUpdates
where ID=AssetID and Definition=StringDefinition and StringID=OldString

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.AssetString(ID, Definition, StringID)
select AssetID, StringDefinition, NewString
from @assetStringUpdates
where NewString is not null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

delete dbo.AssetLongString
from @assetStringUpdates
where ID=AssetID and Definition=LongStringDefinition and StringID=OldLongString

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR

insert dbo.AssetLongString(ID, Definition, StringID)
select AssetID, LongStringDefinition, NewLongString
from @assetStringUpdates
where NewLongString is not null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR


-- assign ranks to generated Epics that are missing them
insert dbo.[Rank](Definition, ID, [Order])
select 'PrimaryWorkitem.Order', PrimaryID, RR.[Order]
from dbo.CustomRelation C 
left join dbo.[Rank] R on R.ID=C.PrimaryID
left join dbo.[Rank] RR on RR.ID=C.ForeignID
where C.Definition='Story.GeneratedFromTheme' and R.[Order] is null

select @rowcount=@@ROWCOUNT, @error=@@ERROR
if @error<>0 goto ERR
print @rowcount + ' epics ranked'


if (@saveChanges = 1) goto OK
raiserror('Rolling back changes.  To commit changes, set @saveChanges=1',16,1)
ERR: rollback tran TX
OK: commit
DONE:

GO

alter table dbo.IDSource drop column IsNew, ThemeID, Name
