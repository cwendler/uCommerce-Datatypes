<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultiProduct.ascx.cs" Inherits="byte5.UCommerceExt.Datatypes.MultiProduct" %>
<asp:UpdateProgress ID="updateProgressMulti" runat="server" AssociatedUpdatePanelID="MultiProductPanel">
	<ProgressTemplate>
		<img src="/umbraco/images/throbber.gif" alt="Loading"/>
	</ProgressTemplate>	
</asp:UpdateProgress>
<script type="text/javascript">
    function postBackByObjectMulti() {
        var o = window.event.srcElement;
        if (o.tagName == "INPUT" && o.type == "checkbox") {
            __doPostBack("", "");
        }
    }
</script>
<asp:UpdatePanel runat="server" ID="MultiProductPanel">
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
                    <asp:ImageButton runat="server" ID="clearMultiSelection" OnClick="clearMultiSelection_Clicked" />
		        </td>
	        </tr>
	        <tr>
		        <td colspan="3">
			        <asp:TreeView id="treeMultiProduct" runat="server" 
				        OnTreeNodeCheckChanged="treeMultiProduct_TreeNodeCheckChanged"
				        OnTreeNodeExpanded="treeMultiProduct_NodeExpanded"
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
				        AutoPostBack="True">
				        <Nodes>
				        </Nodes>
			        </asp:TreeView>
		        </td>
	        </tr>
        </table>
    </ContentTemplate>
</asp:UpdatePanel>
