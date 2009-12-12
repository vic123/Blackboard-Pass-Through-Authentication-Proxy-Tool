﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Idla.PtaProxy.Admin.Register" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Register pass-through authentication proxy server</title>
    <style type="text/css">
        .style1
        {
            font-size: large;
        }
    </style>
</head>
<body>
    <form id="form2" runat="server">
    <div style="height: 1236px; margin-bottom: 4px;">
        Pass Through Authentication Proxy
        Registration With Blackboard Academic Suite Server Page<br />
        <br />
        <asp:Label ID="Label1" runat="server" BorderStyle="None" 
            Text="Blackboard URL (an.academic.suite.server):"></asp:Label>
        <br />
        <br />
        <asp:DropDownList ID="ddlBBoard" runat="server" DataSourceID="dsBBoard" 
            DataTextField="BBoardListValue" DataValueField="BBoardCode" Height="18px" 
            Width="362px" onselectedindexchanged="ddlBBoard_SelectedIndexChanged">
        </asp:DropDownList>
        <br />
        <asp:Button ID="btnRefreshList" runat="server" onclick="btnRefreshList_Click" 
            Text="Refresh" Width="205px" />
        <br />
        <br />
        <asp:Label ID="Label2" runat="server" 
            
            Text="Blackboard Proxy Tool Registration Password (Set at BB Proxy Tools, Global Properties):"></asp:Label>
        <br />
        <asp:TextBox ID="txtBBRegPwd" runat="server" Width="146px" TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnPreviewRegXml" runat="server" 
            Text="Preview Registration XML" Width="200px" 
            onclick="btnPreviewRegXml_Click" />
        <br />
        <asp:TextBox ID="txtRegXml" runat="server" Height="69px" TextMode="MultiLine" 
            Width="642px" style="margin-top: 0px"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnRegister" runat="server" onclick="btnRegister_Click" Text="Register" 
            Width="212px" />
        <asp:Label ID="lblRegisterResult" runat="server"></asp:Label>
        <br />
        <br />
        <br />
        <b><span class="style1">Edit Registrations at Blackboard Servers:</span><br />
        Simplest way to copy sample menu link definitions to new Blackboard registration 
        record is:<br />
        1) Rename existing BBoardCode containing links to new code.<br />
        2) Select it, copy xml from &quot;XML data of BBMenuLink and Param tables&quot; to clipboard.
        <br />
        3) Rename initial BBoardCode back, insert new record and set its BBoardCode to new 
        code.
        <br />
        4) Select newly inserted record, paste preserved xml.</b><br />
        <asp:GridView ID="gvBBoard" runat="server" AllowSorting="True" 
            AutoGenerateColumns="False" AutoGenerateDeleteButton="True" 
            AutoGenerateEditButton="True" AutoGenerateSelectButton="True" 
            DataKeyNames="BBoardCode" DataSourceID="dsBBoard" 
            onselectedindexchanged="gvBBoard_SelectedIndexChanged" 
            onrowupdated="gvBBoard_RowUpdated" onrowupdating="gvBBoard_RowUpdating">
            <Columns>
                <asp:BoundField DataField="BBoardCode" HeaderText="BBoardCode" 
                    SortExpression="BBoardCode" />
                <asp:BoundField DataField="BBoardURL" HeaderText="BBoardURL" 
                    SortExpression="BBoardURL" />
                <asp:BoundField DataField="BBoardHTTPPort" HeaderText="BBoardHTTPPort" 
                    SortExpression="BBoardHTTPPort" />
                <asp:BoundField DataField="BBoardHTTPSPort" HeaderText="BBoardHTTPSPort" 
                    SortExpression="BBoardHTTPSPort" />
                <asp:BoundField DataField="BBoardProxyCode" HeaderText="BBoardProxyCode" 
                    SortExpression="BBoardProxyCode" />
                <asp:BoundField DataField="IsRegistered" 
                    HeaderText="IsRegistered" SortExpression="IsRegistered" />
                <asp:BoundField DataField="SharedSecret" HeaderText="SharedSecret" 
                    SortExpression="SharedSecret" />
                <asp:BoundField DataField="GUID" HeaderText="GUID" 
                    SortExpression="GUID" />
                <asp:BoundField DataField="ProxyState" HeaderText="ProxyState" 
                    SortExpression="ProxyState" />
                <asp:BoundField DataField="OnErrorServerTransfer" HeaderText="OnErrorServerTransfer" 
                    SortExpression="OnErrorServerTransfer" />
                <asp:BoundField DataField="RegisterHttpURL" HeaderText="RegisterHttpURL" 
                    SortExpression="RegisterHttpURL" />
                <asp:BoundField DataField="RegisterHttpsURL" HeaderText="RegisterHttpsURL" 
                    SortExpression="RegisterHttpsURL" />
                <asp:BoundField DataField="RegisterS2SURL" HeaderText="RegisterS2SURL" 
                    SortExpression="RegisterS2SURL" />
                <asp:BoundField DataField="RegisterActions" HeaderText="RegisterActions" 
                    SortExpression="RegisterActions" />
                <asp:BoundField DataField="BBoardHTTPSchema" HeaderText="BBoardHTTPSchema" 
                    SortExpression="BBoardHTTPSchema" />
                <asp:BoundField DataField="Comment" HeaderText="Comment" 
                    SortExpression="Comment" />
                <asp:BoundField DataField="BBoardListValue" HeaderText="BBoardListValue" 
                    ReadOnly="True" SortExpression="BBoardListValue" />
            </Columns>
        </asp:GridView>
        <asp:Button ID="btnBBoardAdd" runat="server" onclick="btnBBoardAdd_Click" 
            Text="Add new record" Width="113px" />
        &nbsp;(Modify BBoardCode, BBoardURL and probably BBoardProxyCode of new record in order to add another one)<br />
        In order to delete BBoard record, first select it, clean up XML, Save XML - this 
        will delete records from dependent BBMenuLink and Param tables.<br />
        <asp:Button 
            ID="btnRefresh" runat="server" onclick="btnRefresh_Click" Text="Refresh" 
            Width="125px" />
        <br />
        BB Registration columns descriprion:<br />
&nbsp;- BBoardCode - unique code of Registration<br />
&nbsp;- BBoardURL - domain name or IP (without protocol, i.e. without
        <a href="http://%20prefix">http:// 
             prefix</a>)<br />
&nbsp;- BBoardProxyCode - code (name) of proxy how it will appear in Program field of 
        Proxy Tools list at Blackboard server. BBoardProxyCode should be unique for 
        particular BBoardURL&nbsp; <br />
        &nbsp;- IsRegistered - Y/N flag indicating current registration status of Proxy 
        at parricular Blackboard server, may be out of sync with Blackboard<br />
&nbsp;- SharedSecret - filled automatically upon registration. Can be manually modified, 
        but this has to be synchronized with Proxy configuration settings on Blackboard 
        side<br />
&nbsp;- GUID - autogenerated by Blackboard and filled upon registration, should not be modified<br />
&nbsp;- ProxyState - state of the Proxy at Blackboard obtained from state-change 
        Blackboard callback action type (possible values - available, inactive, 
        unavailable), may be out of sync with Blackboard<br />
&nbsp;- OnErrorServerTransfer - may contain url where Server.Transfer() will redirect 
        request in case of error during processing of pass-through authentication 
        request. Ignored when set to NULL.<br />
&nbsp;- RegisterHttpURL, RegisterHttpsURL and RegisterS2SURL - Y/N flags controlling 
        what types of entry points Proxy will provide to Blackboard. Links by themselves 
        are defined in pta_proxy\web.config. If the SSL handshake fails then an error is 
        logged and Blackboard tries non-ssl URL for the request. However if https is 
        registered without http it will not be possible, i.e. Blackboard is restricted 
        to use https protocol. If the configuration of the network path from the 
        Blackboard Learn server to the proxy tool server is different than the path from 
        the external world to the proxy tool server then specify a different URL for the 
        Blackboard Learn server to use for the simple XML response requests. This 
        alternate URL is attempted first; if it fails due to an SSL handshake error then 
        Blackboard Learn falls back to the https URL (if configured) and then to the 
        http URL if necessary. ( <a href="http://www.edugarage.com/display/BBDN/Baseurl">
        http://www.edugarage.com/display/BBDN/Baseurl</a> )
        <br />
        According to tests made, when both Http and Https URLs are defined, Blackboard 
        actions are initiated via https and does not fail back to http at (??)least when 
        there is no real server available at specified <a href="URL:port">URL:port</a>. 
        Link redirection is perfomed via http, and no https attempt is performed if &quot;SSL 
        Required&quot; is not checked in Blackboard&#39;s proxy configuration.<br />
        - RegisterActions - Y/N flag controlling whether handling of callback actions 
        will be supported. Actions initiated from Blackboard include such as proxy state 
        change, removing of registration, config, reregister and ping that allow 
        supporting of proxy state information in IsRegistered and ProxyState, remote 
        re-configuration and activation of re-registration, and pinging before 
        redirecting of user. They may cause additional problems during initial Proxy 
        configuration and made possible to be disabled.<br />
&nbsp;- BBoardHTTPSchema - may contain values &#39;HTTP&#39;, &#39;HTTPS&#39; or &#39;BOTH&#39;. This defines 
        protocol(s) used during registration of proxy server. If BOTH is specified, then 
        proxy tries https first and in case of catching of&nbsp; System.Net.WebException 
        with message &quot;The underlying connection was closed: Could not establish trust 
        relationship for the SSL/TLS secure channel.&quot; (may be too strict condition) 
        falls back to http protocol (same logic as used by Blackboard).
        <br />
        - Comment - any comments<br />
        <br />
        <br />
        <b><span class="style1">XML data of BBMenuLink and Param tables:</span></b><br />
        <asp:TextBox ID="txtXML" runat="server" Height="230px" Width="637px" 
            TextMode="MultiLine"></asp:TextBox>
        <br />
        <asp:Button ID="btnSave" runat="server" onclick="btnSave_Click" Text="Save" 
            Width="115px" />
        <asp:Label ID="lblSaveXMLResult" runat="server"></asp:Label>
        <br />
        <br />
        PtaProxy.BBMenuLink defines links that will be registered in Blackboard 
        interface. Fields:
        <br />
&nbsp;- BBLinkPath - the rest of the URL after application path/proxy location/pta/. 
        Should not contain starting &quot;/&quot;. Should follow conventions discussed at II.5. 
        Supplied URL query string parameters are ignored and cut off by Blackboard. Form 
        parameters that can be hardcoded for Actions (i.e. BB callbacks) are ignored for 
        menu links according to test results. The only way to distinguish different 
        links is with their virtual filename or path info.&nbsp;- BBLinkType - &#39;course_tool&#39;, &#39;system_tool&#39;, &#39;user_tool&#39;, &#39;tool&#39; or &#39;group_tool&#39;, 
        check is encoded in DB schema. These are constants accepted by Blackboard that 
        define link location in Blackboard interface. See 
        http://www.edugarage.com/display/BBDN/Link+Type for more details.
        <br />
&nbsp;- BBLinkName - this is link caption, i.e. text shown in Blackboard GIU as link 
        name. Localization settings are not supported. Tested that cyrrilic link for 
        example shows up on test Blackboard server as &quot;????&quot; and responded to clicks (?? 
        not sure).
        <br />
&nbsp;- Comment - any comments on what should be expected passthrough authentication 
        system response on receiving of request on URL defined through BBLinkPath.
        <br />
        <br />
        Please, see readme IV. Proxy registration and application source code for explanation/examination of link 
        transformation and redirection rules stored in Param table.<br />
        <br />
        Default pta_admin password is pta_admin<br />
        <asp:Button ID="btnCreatePtaAdmin" runat="server" 
            onclick="btnCreatePtaAdmin_Click" Text="Create pta_admin user" />
        <asp:Label ID="lblCreatePtaAdmin" runat="server"></asp:Label>
        <br />
        pta_admin old password:         <asp:TextBox ID="txtPtaOldPwd" runat="server" TextMode="Password"></asp:TextBox>
&nbsp;new password:
        <asp:TextBox ID="txtPtaNewPwd" runat="server" TextMode="Password"></asp:TextBox>
&nbsp;<asp:Button ID="btnSetPassword" runat="server" onclick="btnSetPassword_Click" 
            Text="Set Password" />
        <asp:Label ID="lblSetPassword" runat="server"></asp:Label>
        <br />
        <br />
        <asp:SqlDataSource ID="dsBBoard" runat="server" 
            ConnectionString="<%$ ConnectionStrings:PtaProxySQLConnectionString %>" 
            DeleteCommand="DELETE FROM BBoard WHERE (BBoardCode = @BBoardCode)" 
            InsertCommand="INSERT INTO BBoard DEFAULT VALUES" 
            SelectCommand="SELECT *, BBoardCode + '(' + BBoardURL + ' ' + BBoardProxyCode + ')' as BBoardListValue FROM [BBoard] ORDER BY [BBoardCode]" 
            UpdateCommand="UPDATE BBoard SET BBoardCode = @NewBBoardCode,
BBoardURL = @BBoardURL,
BBoardHttpPort = @BBoardHttpPort,
BBoardHttpsPort = @BBoardHttpsPort,
      BBoardProxyCode = @BBoardProxyCode,
      IsRegistered = @IsRegistered,
      SharedSecret = @SharedSecret,
      GUID = @GUID,
      ProxyState = @ProxyState,
      OnErrorServerTransfer = @OnErrorServerTransfer,
      RegisterHttpURL = @RegisterHttpURL,
      RegisterHttpsURL = @RegisterHttpsURL,
      RegisterS2SURL = @RegisterS2SURL,
      RegisterActions = @RegisterActions,
      BBoardHTTPSchema = @BBoardHTTPSchema,
      Comment = @Comment
 WHERE (BBoardCode = @OldBBoardCode)" onupdating="dsBBoard_Updating">
            <DeleteParameters>
                <asp:ControlParameter ControlID="gvBBoard" Name="BBoardCode" 
                    PropertyName="SelectedValue" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="gvBBoard" Name="BBoardProxyCode" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="BBoardURL" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="IsRegistered" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="SharedSecret" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="GUID" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="ProxyState" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="OnErrorServerTransfer" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="RegisterHttpURL" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="RegisterHttpsURL" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="RegisterS2SURL" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="RegisterActions" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="BBoardHTTPSchema" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="Comment" 
                    PropertyName="SelectedValue" />
                <asp:Parameter Name="OldBBoardCode" />
                <asp:Parameter Name="NewBBoardCode" />
                <asp:ControlParameter ControlID="gvBBoard" Name="BBoardHttpPort" 
                    PropertyName="SelectedValue" />
                <asp:ControlParameter ControlID="gvBBoard" Name="BBoardHttpsPort" 
                    PropertyName="SelectedValue" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    </form>
</body>
</html>
