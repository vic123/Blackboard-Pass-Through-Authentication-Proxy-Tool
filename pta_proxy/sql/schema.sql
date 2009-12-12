--Warnings raised during tables creation like:
	--Warning! The maximum key length is 900 bytes. The index 'XIF2Param' has maximum length of 4096 bytes. For some combination of large values, the insert/update operation will fail.
	--are not something dangerous - it is unlikely that all key/index fields to contain so much data, 
	--but their datatypes are defined to be large, allowing for example one of them to contain much data upon necessity

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.Param') AND type in (N'U'))
DROP TABLE dbo.Param
go

CREATE TABLE Param (
	BBoardCode			nvarchar(255) NOT NULL DEFAULT 'NEW_BB_CODE',
	BBLinkPath			nvarchar(1024) NOT NULL,
	ParamTag			varchar(255) NOT NULL CHECK (ParamTag LIKE '{%}'),
	ParamOrderCode		varchar(255) NOT NULL,
	ParamInputType		varchar(255) NOT NULL
					CHECK (ParamInputType IN ('Param.InputTemplate', 'ProxyLinkPath', 'ProxyLinkType', 'ProxyPath', 'Request.ApplicationPath', 'Request.FilePath', 'Request.Form', 'Request.InputStream', 'Request.QueryString', 'Request.PathInfo', 'Request.Url.ToString', 'SProcCall', 'BBWSCall')),
	ParamOutputType		varchar(255) NOT NULL
					CHECK (ParamOutputType IN ('FormsAuthentication.SetAuthCookie', 'Context.Items', 'Context.Session', 'Response.AppendHeader', 'Response.Output', 'Response.Status', 'Server.Transfer', 'None')),
	ParamInputTemplate	nvarchar(4000) NULL,
	ParamKey			nvarchar(255) NULL,
	RegExp				nvarchar(1024) NULL,
	RegExpOperation		varchar(255) NOT NULL
					CHECK (RegExpOperation IN ('Assign', 'REMatch', 'REReplace')),
	RegExpReplaceString	nvarchar(1024) NULL,
	RegExpMatchGroup	smallint NULL,
	RegExpMatchCapture	smallint NULL,
	SProcName			nvarchar(255) NULL,
	Comment				nvarchar(4000) NULL
)
go

CREATE UNIQUE NONCLUSTERED INDEX Param_OrderCode ON dbo.Param 
(
	BBoardCode ASC,
	BBLinkPath ASC,
	ParamOrderCode ASC
)
go


ALTER TABLE Param
	ADD PRIMARY KEY CLUSTERED (BBoardCode ASC, BBLinkPath ASC, ParamTag ASC)
go



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.BBMenuLink') AND type in (N'U'))
DROP TABLE dbo.BBMenuLink
go
CREATE TABLE BBMenuLink (
	BBoardCode			nvarchar(255) NOT NULL DEFAULT 'NEW_BB_CODE',
	BBLinkPath			nvarchar(1024) NOT NULL,
	BBLinkType			varchar(255) NOT NULL
					CHECK (BBLinkType IN ('course_tool', 'system_tool', 'user_tool', 'tool', 'group_tool')),
	BBLinkName			nvarchar(255) NOT NULL,
	BBLinkIconPath		nvarchar(1024) NULL,
	Enabled				char(1) NOT NULL
					CHECK (Enabled IN ('Y', 'N')) DEFAULT 'Y',

	Comment				nvarchar(4000) NULL,
)
go

ALTER TABLE BBMenuLink
	ADD PRIMARY KEY CLUSTERED (BBoardCode ASC, BBLinkPath ASC)
go

--INSERT INTO BBoard DEFAULT VALUES

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.BBoard') AND type in (N'U'))
DROP TABLE dbo.BBoard
go
CREATE TABLE BBoard (
		BBoardCode				nvarchar(255) NOT NULL DEFAULT 'NEW_BB_CODE',
		BBoardURL				nvarchar(1024) NOT NULL DEFAULT 'NEW_URL',
		BBoardHTTPPort			smallint NOT NULL DEFAULT 80,
		BBoardHTTPSPort			smallint NOT NULL DEFAULT 443,
		BBoardProxyCode			nvarchar(255) NOT NULL DEFAULT 'pta_proxy',
		IsRegistered			char(1) NOT NULL	CHECK (IsRegistered IN ('Y', 'N')) DEFAULT 'N',
		SharedSecret			varchar(1024) NULL,
		GUID					varchar(1024) NULL,
		ProxyState				varchar(255) NULL  CHECK (ProxyState IN ('available', 'inactive', 'unavailable')) DEFAULT 'unavailable',
		OnErrorServerTransfer	nvarchar(1024) NULL,
		RegisterHttpURL			char(1) NOT NULL	CHECK (RegisterHttpURL IN ('Y', 'N')) DEFAULT 'Y',
		RegisterHttpsURL		char(1) NOT NULL	CHECK (RegisterHttpsURL IN ('Y', 'N')) DEFAULT 'Y',
		RegisterS2SURL			char(1) NOT NULL	CHECK (RegisterS2SURL IN ('Y', 'N')) DEFAULT 'N', 
		RegisterActions			char(1) NOT NULL	CHECK (RegisterActions IN ('Y', 'N')) DEFAULT 'Y', 
		BBoardHTTPSchema		varchar(255) CHECK (BBoardHTTPSchema IN ('HTTP', 'HTTPS', 'BOTH')) DEFAULT 'BOTH',
		Comment					nvarchar(4000) NULL
)
go

CREATE UNIQUE NONCLUSTERED INDEX BBoard_URLProxyCode ON dbo.BBoard 
(
	BBoardURL ASC,
	BBoardProxyCode ASC
)


ALTER TABLE BBoard
	ADD PRIMARY KEY CLUSTERED (BBoardCode ASC)
go


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SampleUser') AND type in (N'U'))
DROP TABLE dbo.SampleUser

CREATE TABLE SampleUser (
	UserId		 nvarchar(255) NOT NULL,
	UserName		nvarchar(255) NOT NULL
)
go


ALTER TABLE SampleUser
	ADD PRIMARY KEY CLUSTERED (UserId ASC)
go


ALTER TABLE Param
	ADD FOREIGN KEY (BBoardCode, BBLinkPath)
				 REFERENCES BBMenuLink  (BBoardCode, BBLinkPath)
				 ON UPDATE CASCADE
go


ALTER TABLE BBMenuLink
	ADD FOREIGN KEY (BBoardCode)
				 REFERENCES BBoard  (BBoardCode)
				 ON UPDATE CASCADE
go



IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.TestAuthSProc') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.TestAuthSProc
go 

create proc TestAuthSProc (@UserId nvarchar(255), @CourseId nvarchar(255), @TCRole nvarchar(255), @UserName nvarchar(255) output) 
as begin
	SELECT @UserName = 'U:' + isNull(@UserId, 'NULL') + '/C:' + isNull(@CourseId, 'NULL') + '/TCR:' + isNull(@TCRole, 'NULL')
--	raiserror ('Invalid user id', 16, 1)
end
go
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.TestRaiseSProc') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.TestRaiseSProc
go
create proc TestRaiseSProc  
as begin
	raiserror ('Invalid user id', 16, 1)
end
go
/*test:
declare @s nvarchar(255)
exec TestAuthSProc 'dsfgd', 'jghj', 'ewrwe', @s output
select @s
*/


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.SetBBMenuLinkParamXML') AND type in (N'P', N'PC'))
DROP PROCEDURE dbo.SetBBMenuLinkParamXML
go
create proc SetBBMenuLinkParamXML (@BBoardCode nvarchar(255), @Xml nvarchar(max)) 
as begin
	DECLARE @err_num int, @msg VARCHAR(max), @sev INT, @st INT
	select @Xml = '<ROOT>' + @Xml + '</ROOT>'
	declare @xml_h int
	exec sp_xml_preparedocument @hdoc = @xml_h OUTPUT, @xmltext = @Xml

	begin try 
		begin tran 
			delete from Param where BBoardCode = @BBoardCode
			delete from BBMenuLink where BBoardCode = @BBoardCode

			insert into BBMenuLink (BBoardCode, BBLinkPath ,BBLinkType ,BBLinkName, BBLinkIconPath, Enabled, Comment)
				SELECT	 * 
				FROM	OPENXML (@xml_h, '/ROOT/BBMenuLink',1)
							WITH (BBoardCode  nvarchar(255),
								  BBLinkPath nvarchar(1024),
								BBLinkType varchar(255),
								BBLinkName nvarchar(255),
								BBLinkIconPath nvarchar(1024),
								Enabled char(1),
								Comment nvarchar(4000))

			insert into Param (BBoardCode, BBLinkPath ,ParamTag ,ParamOrderCode ,ParamInputType,
				ParamOutputType, ParamInputTemplate, ParamKey, RegExp, RegExpOperation, RegExpReplaceString,
				RegExpMatchGroup,RegExpMatchCapture,SProcName,Comment)
				SELECT	 *
				FROM	OPENXML (@xml_h, '/ROOT/BBMenuLink/Param',1)
							WITH (
						BBoardCode			nvarchar(255),
						BBLinkPath			nvarchar(1024),
						ParamTag		varchar(255),
						ParamOrderCode	varchar(255),
						ParamInputType	varchar(255),
						ParamOutputType	varchar(255),
						ParamInputTemplate	nvarchar(4000),
						ParamKey		nvarchar(255),
						RegExp		 nvarchar(1024),
						RegExpOperation	varchar(255),
						RegExpReplaceString  nvarchar(1024),
						RegExpMatchGroup	  smallint,
						RegExpMatchCapture	smallint,
						SProcName		  nvarchar(255),
						Comment		nvarchar(4000)
				)
		commit
	end try
	begin catch 
		SELECT @err_num = ERROR_NUMBER(), @msg = ERROR_MESSAGE(), @sev = ERROR_SEVERITY(), @st = ERROR_STATE()
		rollback
		if @st = 0 set @st = 1
		SELECT  @msg = convert (varchar(50), @err_num) + '; ' + @msg
		--SELECT @msg, @sev, @st
		RAISERROR (@msg, @sev, @st)
	END CATCH
end
go
/* test:
exec SetBBMenuLinkParamXML @BBoardURL = 'idlatest.com', @Xml = '<BBMenuLink BBoardURL="idlatest.com" BBLinkPath="course_tool1.aspx" BBLinkType="course_tool" BBLinkName="Course Tool #1 123">
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool1.aspx" ParamTag="{AppUrl}" ParamOrderCode="010" ParamInputType="Request.ApplicationPath" ParamOutputType="None" RegExpOperation="Assign"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool1.aspx" ParamTag="{HardCodedAuth}" ParamOrderCode="060" ParamInputType="Param.InputTemplate" ParamOutputType="FormsAuthentication.SetAuthCookie" ParamInputTemplate="testuser_123" RegExpOperation="Assign"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool1.aspx" ParamTag="{Response.Location}" ParamOrderCode="050" ParamInputType="Param.InputTemplate" ParamOutputType="Response.AppendHeader" ParamInputTemplate="{AppUrl}TestRedirect/InfoTest.aspx" ParamKey="Location" RegExpOperation="Assign"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool1.aspx" ParamTag="{Response.Status}" ParamOrderCode="055" ParamInputType="Param.InputTemplate" ParamOutputType="Response.Status" ParamInputTemplate="301 Found" RegExpOperation="Assign"/>
</BBMenuLink>
<BBMenuLink BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" BBLinkType="course_tool" BBLinkName="Course Tool #2 - Full Test">
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{AppPath}" ParamOrderCode="020" ParamInputType="Request.ApplicationPath" ParamOutputType="None" RegExpOperation="Assign" Comment="Get Request.ApplicationPath"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{CourseId}" ParamOrderCode="056" ParamInputType="Request.Form" ParamOutputType="Context.Session" ParamKey="course_id" RegExp="(.+)" RegExpOperation="REReplace" RegExpReplaceString="my_course_prefix_$1" Comment="Get Request.Form&quot;course_id&quot;"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{HostUrl}" ParamOrderCode="010" ParamInputType="Request.Url.ToString" ParamOutputType="None" RegExp="^^:/\?#+://^/\?#*" RegExpOperation="REMatch" RegExpMatchGroup="0" RegExpMatchCapture="0" Comment="Test Request.Url.ToString and REMatch"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{InputStream}" ParamOrderCode="060" ParamInputType="Request.InputStream" ParamOutputType="Context.Session" RegExpOperation="Assign" Comment="Get Request.InputStream"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{ProxyLinkPath}" ParamOrderCode="030" ParamInputType="ProxyLinkPath" ParamOutputType="None" RegExpOperation="Assign" Comment="Get ProxyLinkPath"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{ProxyLinkType}" ParamOrderCode="040" ParamInputType="ProxyLinkType" ParamOutputType="None" RegExpOperation="Assign" Comment="Get ProxyLinkType"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{ProxyPath}" ParamOrderCode="025" ParamInputType="ProxyPath" ParamOutputType="Context.Session" ParamKey="ProxyPath" RegExpOperation="Assign"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{Response.Location}" ParamOrderCode="210" ParamInputType="Param.InputTemplate" ParamOutputType="Response.AppendHeader" ParamInputTemplate="{HostUrl}{AppPath}/TestRedirect/InfoTest.aspx?UserId={UserId}&amp;CourseId={CourseId}" ParamKey="Location" RegExpOperation="Assign" Comment=" Set redirect location"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{Response.Output}" ParamOrderCode="110" ParamInputType="Param.InputTemplate" ParamOutputType="Response.Output" ParamInputTemplate="UserId: {UserId}; CourseId: {CourseId}; TCRole: {TCRole}" RegExpOperation="Assign" SProcName="TestAuthSProc" Comment="Assign Param.InputTemplate to Response.Output"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{Response.Status}" ParamOrderCode="220" ParamInputType="Param.InputTemplate" ParamOutputType="Response.Status" ParamInputTemplate="301 Found" RegExpOperation="Assign" Comment="Set redirect status code"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{ReturnURL}" ParamOrderCode="050" ParamInputType="Request.Form" ParamOutputType="Context.Session" ParamKey="returnurl" RegExpOperation="Assign" Comment="Get Request.Form&quot;returnurl&quot;"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{SProcCall}" ParamOrderCode="100" ParamInputType="SProcCall" ParamOutputType="FormsAuthentication.SetAuthCookie" ParamInputTemplate="{UserId}, {CourseId}, {TCRole}" ParamKey="UserName" RegExpOperation="Assign" SProcName="TestAuthSProc" Comment="Get SProcCall"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{TCRole}" ParamOrderCode="054" ParamInputType="Request.Form" ParamOutputType="Context.Session" ParamKey="tcrole" RegExpOperation="Assign" Comment="Get Request.Form&quot;tcrole&quot;"/>
<Param BBoardURL="idlatest.com" BBLinkPath="course_tool2_full_test.aspx" ParamTag="{UserId}" ParamOrderCode="052" ParamInputType="Request.Form" ParamOutputType="Context.Items" ParamKey="userid" RegExpOperation="Assign" Comment="Get Request.Form&quot;userid&quot;"/>
</BBMenuLink>'
*/

