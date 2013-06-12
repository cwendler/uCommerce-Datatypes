<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Product.ascx.cs" Inherits="byte5.UCommerceExt.Datatypes.Product" %>
<asp:UpdateProgress ID="updateProgressProduct"
	runat="server"
	AssociatedUpdatePanelID="ProductPanel">
	<ProgressTemplate>
		<img src="/umbraco/images/throbber.gif" alt="Loading"/>
	</ProgressTemplate>	
</asp:UpdateProgress>
<asp:UpdatePanel runat="server" ID="ProductPanel">
    <ContentTemplate>
        <table cellspacing="20">
	        <tr>
		        <td style="vertical-align: top; width: 30px">
		            <asp:Image ID="selectedProductIcon" runat="server" />
                </td>
                <td style="vertical-align: top; width: auto; padding-top: 7px">
		            <asp:Label ID="selectedProductLink" runat="server" ForeColor="Black" />
                </td>
                <td style="vertical-align: top; padding-top: 6px; width: 20px">
                    <asp:ImageButton runat="server" ID="clearProductSelection" OnClick="clearProductSelection_Clicked" />
		        </td>
	        </tr>
	        <tr>
		        <td colspan="3">
			        <asp:TreeView id="treeProduct" runat="server" 
				        OnSelectedNodeChanged="treeProduct_SelectedNodeChanged"
				        OnTreeNodeExpanded="treeProduct_NodeExpanded"
				        RootNodeStyle-ForeColor="DarkGray"
                        RootNodeStyle-Font-Bold="True"
				        NodeStyle-ForeColor="DarkGray"
				        NodeStyle-HorizontalPadding="5"
				        LeafNodeStyle-ForeColor="Black"
				        SelectedNodeStyle-ForeColor="Black"
				        SelectedNodeStyle-BorderStyle="Solid"
				        SelectedNodeStyle-BorderWidth="1"
				        SelectedNodeStyle-BorderColor="LightSkyBlue"
				        SelectedNodeStyle-BackColor="LightCyan"
				        ExpandDepth="0"
				        AutoPostBack="True"
                        EnableClientScript="False">
				        <Nodes>
				        </Nodes>
			        </asp:TreeView>
		        </td>
	        </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>