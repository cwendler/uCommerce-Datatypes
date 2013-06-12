<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Category.ascx.cs" Inherits="byte5.UCommerceExt.Datatypes.Category" %>
<asp:UpdateProgress ID="UpdateProgress1"
	runat="server"
	AssociatedUpdatePanelID="CategoryPanel">
	<ProgressTemplate>
		<img src="/umbraco/images/throbber.gif" alt="Loading"/>
	</ProgressTemplate>	
</asp:UpdateProgress>
<asp:UpdatePanel runat="server" UpdateMode="Always" ID="CategoryPanel">
    <ContentTemplate>
        <table cellspacing="20">
	        <tr>
		        <td style="vertical-align: top; width: 30px">
		            <asp:Image ID="selectedCategoryIcon" runat="server" />
                </td>
                <td style="vertical-align: top; width: auto; padding-top: 7px">
		            <asp:Label ID="selectedCategoryLabel" runat="server" ForeColor="Black" />
                </td>
                <td style="vertical-align: top; padding-top: 6px; width: 20px">
                    <asp:ImageButton runat="server" ID="clearSelection" OnClick="clearSelection_Clicked" />
		        </td>
	        </tr>
	        <tr>
		        <td colspan="3">
			        <asp:TreeView id="treeCategory" runat="server" 
				        OnSelectedNodeChanged="treeCategory_SelectedNodeChanged"
				        OnTreeNodeExpanded="treeCategory_NodeExpanded"
				        RootNodeStyle-ForeColor="DarkGray"
                        RootNodeStyle-Font-Bold="True"
				        NodeStyle-ForeColor="Black"
				        NodeStyle-HorizontalPadding="5"
				        SelectedNodeStyle-ForeColor="Black"
				        SelectedNodeStyle-BorderStyle="Solid"
				        SelectedNodeStyle-BorderWidth="1"
				        SelectedNodeStyle-BorderColor="LightSkyBlue"
				        SelectedNodeStyle-BackColor="LightCyan"
				        ExpandDepth="0"
				        AutoPostBack="True">
				        <Nodes>
				        </Nodes>
			        </asp:TreeView>
		        </td>
	        </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
