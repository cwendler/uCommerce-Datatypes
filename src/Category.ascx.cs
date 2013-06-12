using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UCommerce.EntitiesV2;

namespace byte5.UCommerceExt.Datatypes {

	public partial class Category : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor {

		private string _umbracoValue;

		protected void Page_Load(object sender, EventArgs e) {

			if(!IsPostBack) {
                clearSelection.ImageUrl = "/umbraco/images/close.png";
				var catalogs = from catalog in UCommerce.EntitiesV2.ProductCatalog.All() select catalog;

				// Anfangen den TreeView aufzubauen, die obersten Knoten bilden die Kataloge
				foreach(UCommerce.EntitiesV2.ProductCatalog productCatalog in catalogs) {
					TreeNode tn = new TreeNode(productCatalog.Name, productCatalog.ProductCatalogId.ToString());
					tn.ImageUrl = "/umbraco/images/umbraco/folder.gif";

					//Den Knoten werden dann die obersten Kategorien als Unterknoten hinzugefügt
					if(productCatalog.Categories.Count(cat => cat.ParentCategory == null) > 0) {
						tn.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
					}
					treeCategory.Nodes.Add(tn);
				}
				Session["ucommerce_category_control_" + this.UniqueID] = _umbracoValue;
			}
			else {
				_umbracoValue = (string)Session["ucommerce_category_control_" + this.UniqueID];
			}

			if(string.IsNullOrEmpty(_umbracoValue)) {
                selectedCategoryIcon.ImageUrl = "/umbraco/images/speechBubble/error.png";
                selectedCategoryLabel.Text = "";
                clearSelection.Visible = false;
			}
			else {
			    if(treeCategory.SelectedNode != null) {
			        selectedCategoryIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
			        selectedCategoryLabel.Text = treeCategory.SelectedNode.Text;
                    clearSelection.Visible = true;
			    }
		        else {
					int categoryId = int.Parse(_umbracoValue);
					var selectedCategory = UCommerce.EntitiesV2.Category.All().SingleOrDefault(cat => cat.CategoryId == categoryId);
					if (selectedCategory != null) {
                        selectedCategoryIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
						selectedCategoryLabel.Text = selectedCategory.Name;
                        clearSelection.Visible = true;
					}
                    else {
                        selectedCategoryIcon.ImageUrl = "/umbraco/images/speechBubble/error.png";
					    selectedCategoryLabel.Text = "";
                        clearSelection.Visible = false;
					}
				}
			}
		}

		public object value {
			get {
				if(treeCategory.SelectedNode != null)
					return treeCategory.SelectedNode.Value;
				return _umbracoValue;
			}
			set {
				if(value != null)
					_umbracoValue = value.ToString();
				else
					_umbracoValue = "";
			}
		}

		/// <summary>
		/// Überprüft, ob der selektierte Knoten geändert wurde und achtet darauf, dass unerlaubte Knoten wieder deselektiert werden.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void treeCategory_SelectedNodeChanged(object sender, EventArgs e) {
            
			if(treeCategory.SelectedNode.Depth != 0) {
				_umbracoValue = treeCategory.SelectedNode.Value;
				Session["ucommerce_category_control_" + this.UniqueID] = _umbracoValue; 
                selectedCategoryIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
                selectedCategoryLabel.Text = treeCategory.SelectedNode.Text;
                clearSelection.Visible = true;
			}
			else {
				if(Session["ucommerce_category_control_" + this.UniqueID] != null) {
					_umbracoValue = Session["ucommerce_category_control_" + this.UniqueID].ToString();

					//Durch alle Knoten rekursiv durchiterieren
					foreach(TreeNode tn in treeCategory.Nodes) {
						if(tn.Value == _umbracoValue) {	
                            tn.Select();
							break;
						}
						if(tn.ChildNodes.Count > 0) {
							CheckSubNodesForSelection(tn, _umbracoValue);
						}
					}
                    
                    selectedCategoryIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
				    selectedCategoryLabel.Text = treeCategory.SelectedNode.Text;
                    clearSelection.Visible = true;
				}
                else {
				    _umbracoValue = "";
                    selectedCategoryIcon.ImageUrl = "/umbraco/images/speechBubble/error.png";
                    selectedCategoryLabel.Text = "";
                    clearSelection.Visible = false;
				}           
			}
		}

		protected void treeCategory_NodeExpanded(object sender, TreeNodeEventArgs e) {

			if (e.Node.ChildNodes.Count > 0 && e.Node.ChildNodes[0].Value == "NULL_NODE") {
				e.Node.ChildNodes.RemoveAt(0);
				if (e.Node.Depth == 0) {
					int catalogId = int.Parse(e.Node.Value);
					var catalog = UCommerce.EntitiesV2.ProductCatalog.All().SingleOrDefault(cata => cata.ProductCatalogId == catalogId);
					if (catalog != null) {
						foreach (var category in catalog.Categories.Where(cate => cate.ParentCategory == null)) {
							TreeNode catNode = new TreeNode(category.Name, category.CategoryId.ToString());
							catNode.ImageUrl = "/umbraco/images/umbraco/folder.gif";
							e.Node.ChildNodes.Add(catNode);
							if (category.Categories.Count > 0) {
								catNode.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
							}
							if(catNode.Value == _umbracoValue)
								catNode.Select();
						}
					}
				}
				if(e.Node.Depth > 0) {
					int categoryId = int.Parse(e.Node.Value);
					var category = UCommerce.EntitiesV2.Category.All().SingleOrDefault(cate => cate.CategoryId == categoryId);
					if(category != null) {
						foreach(var subCat in category.Categories) {
							TreeNode catNode = new TreeNode(subCat.Name, subCat.CategoryId.ToString());
							catNode.ImageUrl = "/umbraco/images/umbraco/folder.gif";
							e.Node.ChildNodes.Add(catNode);
							if(subCat.Categories.Count > 0) {
								catNode.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
							}
							if(catNode.Value == _umbracoValue)
								catNode.Select();
						}
					}
				}
			}
		}

		/// <summary>
		/// Iteriert rekursiv durch alle Kind-Knoten eines TreeNode und sucht nach dem Name den zu selektierenden Knoten
		/// </summary>
		/// <param name="treeNode">Der TreeNode, durch dessen ChildNodes iteriert werden soll</param>
		/// <param name="valueToSelect">Der Kategorie-Name, nach dem gesucht wird</param>
		private void CheckSubNodesForSelection(TreeNode treeNode, string valueToSelect) {

			foreach(TreeNode tn in treeNode.ChildNodes) {
				if(tn.Value == valueToSelect) {
					tn.Select();
					break;
				}
				if(tn.ChildNodes.Count > 0) {
					CheckSubNodesForSelection(tn, valueToSelect);
				}
			}

		}

        protected void clearSelection_Clicked(object sender, EventArgs e) {
            _umbracoValue = "";
            Session["ucommerce_category_control_" + this.UniqueID] = null; 
            treeCategory.CollapseAll();
            selectedCategoryIcon.ImageUrl = "/umbraco/images/speechBubble/error.png";
            selectedCategoryLabel.Text = "";
            clearSelection.Visible = false;
        }

	}

}