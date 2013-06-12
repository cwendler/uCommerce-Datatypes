using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace byte5.UCommerceExt.Datatypes {

    public partial class Product : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor {

        string _umbracoValue;

		private string catalogPrefix = "catalog";
    	private string categoryPrefix = "category";
		private string productPrefix = "product";

        protected void Page_Load(object sender, EventArgs e) {

			if(!IsPostBack) {
			    clearProductSelection.ImageUrl = "/umbraco/images/close.png";
			    InitTree();
			}
			else {
                if(Session["ucommerce_product_control_" + this.UniqueID] != null)
				    _umbracoValue = (string)Session["ucommerce_product_control_" + this.UniqueID];
			}

			if(string.IsNullOrEmpty(_umbracoValue)) {
                selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/error.png";
				selectedProductLink.Text = "";
			    clearProductSelection.Visible = false;
			}
			else {
				TreeNode selectedNode = treeProduct.SelectedNode;
				if (selectedNode != null) {
					string[] val = selectedNode.Value.Split('|');
					_umbracoValue = val[1];
                    selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
					selectedProductLink.Text = selectedNode.Text;
				    clearProductSelection.Visible = true;
					TreeNode parentNode = selectedNode.Parent;
					while (parentNode != null) {
						selectedProductLink.Text = parentNode.Text + "/" + selectedProductLink.Text;
						parentNode = parentNode.Parent;
					}
				}
				else {
					var product = UCommerce.EntitiesV2.Product.All().SingleOrDefault(p => p.Sku == _umbracoValue && (p.VariantSku == "" || p.VariantSku == null ));
					if (product != null) {
                        selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
						selectedProductLink.Text = GetPathString(product);
					    clearProductSelection.Visible = true;
					}
				}
			}

        }

        public object value {
            get {
				if(treeProduct.SelectedNode != null) {
					string[] val = treeProduct.SelectedNode.Value.Split('|');
					if (val[0] == "product")
						return val[1];
				}
            	return _umbracoValue;
            }
            set {
				if(value != null) {
					_umbracoValue = value.ToString();
				}
				else
					_umbracoValue = "";
            }
        }

        private void InitTree() {
            var catalogs = from catalog in UCommerce.EntitiesV2.ProductCatalog.All() select catalog;

            // Anfangen den TreeView aufzubauen. Die obersten Knoten bilden die Kataloge
            foreach(UCommerce.EntitiesV2.ProductCatalog productCatalog in catalogs) {
                TreeNode catalogNode = new TreeNode(productCatalog.Name, catalogPrefix + "|" + productCatalog.ProductCatalogId.ToString());
                catalogNode.ImageUrl = "/umbraco/images/umbraco/folder.gif";
                catalogNode.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
                treeProduct.Nodes.Add(catalogNode);
            }

            Session["ucommerce_product_control_" + this.UniqueID] = _umbracoValue;           
        }
		
		/// <summary>
		/// Überprüft, ob der selektierte Knoten geändert wurde und achtet darauf, dass unerlaubte Knoten wieder deselektiert werden.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void treeProduct_SelectedNodeChanged(object sender, EventArgs eventArgs) {

			string[] val = treeProduct.SelectedNode.Value.Split('|');
			// Wenn der Selektierte Knoten eine SKU gespeichert hat, wird er als Produkt angesehen und der Wert einfach gespeichert.
			if(val[0] == productPrefix) {
				_umbracoValue = val[1];
				Session["ucommerce_product_control_" + this.UniqueID] = _umbracoValue;
                selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
                selectedProductLink.Text = treeProduct.SelectedNode.Text;
                clearProductSelection.Visible = true;
                TreeNode parentNode = treeProduct.SelectedNode.Parent;
                while(parentNode != null) {
                    selectedProductLink.Text = parentNode.Text + "/" + selectedProductLink.Text;
                    parentNode = parentNode.Parent;
                }
			}
			//Wenn der selektierte Knoten keine SKU hat:
			else {
				treeProduct.SelectedNode.Selected = false;

				if(Session["ucommerce_product_control_" + this.UniqueID] != null) {
					_umbracoValue = Session["ucommerce_product_control_" + this.UniqueID].ToString();

					//Durch alle Knoten durchiterieren, um den alten Knoten wieder zu selektieren
					foreach(TreeNode node in treeProduct.Nodes) {

						if(node.Value == productPrefix + "|" + _umbracoValue) {
							node.Select();
							break;
						}
						//Falls der Knoten noch nicht gefunden wurde, auch durch die Kind-Knoten durchiterieren.
						else if (node.ChildNodes.Count > 0) {
							CheckSubNodesForSelection(node, productPrefix + "|" + _umbracoValue);
						}

					}

				}

                selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
                selectedProductLink.Text = treeProduct.SelectedNode.Text;
                clearProductSelection.Visible = true;
                TreeNode parentNode = treeProduct.SelectedNode.Parent;
                while(parentNode != null) {
                    selectedProductLink.Text = parentNode.Text + "/" + selectedProductLink.Text;
                    parentNode = parentNode.Parent;
                }
			}

		}

		protected void treeProduct_NodeExpanded(object sender, TreeNodeEventArgs tnea) {

			TreeNode n = tnea.Node;
			if (n.ChildNodes[0].Value == "NULL_NODE") {
				n.ChildNodes.RemoveAt(0);
				string[] val = n.Value.Split('|');
				if (val[0] == catalogPrefix) {
					int catalogId = int.Parse(val[1]);
					var catalog = UCommerce.EntitiesV2.ProductCatalog.All().SingleOrDefault(cata => cata.ProductCatalogId == catalogId);
					if (catalog != null) {
						foreach (var category in catalog.Categories.Where(cate => cate.ParentCategory == null)) {
							TreeNode catNode = new TreeNode(category.Name, categoryPrefix + "|" + category.CategoryId.ToString())
							                   	{ImageUrl = "/umbraco/images/umbraco/folder.gif"};
							if (category.Categories.Count > 0 || category.Products.Count() > 0) {
								catNode.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
							}
							n.ChildNodes.Add(catNode);
						}
					}
				}
				else if (val[0] == categoryPrefix) {
					int categoryId = int.Parse(val[1]);
					var category = UCommerce.EntitiesV2.Category.All().SingleOrDefault(cate => cate.CategoryId == categoryId);
					if (category != null) {
						foreach (var subCat in category.Categories) {
							TreeNode catNode = new TreeNode(subCat.Name, categoryPrefix + "|" + subCat.CategoryId.ToString())
							                   	{ImageUrl = "/umbraco/images/umbraco/folder.gif"};
							if (subCat.Categories.Count > 0 || subCat.Products.Count() > 0) {
								catNode.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
							}
							n.ChildNodes.Add(catNode);
						}
						foreach (var product in category.Products) {
							TreeNode prodNode = new TreeNode(product.Name, productPrefix + "|" + product.Sku)
							                    	{ImageUrl = "/umbraco/images/umbraco/package.gif"};
							n.ChildNodes.Add(prodNode);
							if (product.Sku == _umbracoValue) {
								prodNode.Select();
							}
						}

					}
				}
			}

		}

    	/// <summary>
		/// Iteriert rekursiv durch alle Kind-Knoten eines TreeNode und sucht nach der SKU den zu selektierenden Knoten
		/// </summary>
		/// <param name="treeNode">Der TreeNode, durch dessen ChildNodes iteriert werden soll</param>
		/// <param name="valueToSelect">Die SKU, nach der gesucht wird</param>
		protected void CheckSubNodesForSelection(TreeNode treeNode, string valueToSelect) {

			foreach(TreeNode tn in treeNode.ChildNodes) {
				if(tn.Value == valueToSelect) {
					tn.Select();
					break;
				}
				else if(tn.ChildNodes.Count > 0) {
					CheckSubNodesForSelection(tn, valueToSelect);
				}
			}

		}

		private string GetPathString(UCommerce.EntitiesV2.Product product) {

			string path = product.Name;
			var parentCategories = product.GetCategories();
			if (parentCategories.Count > 0)
			{
				path = GetPathString(parentCategories[0]) + "/" + path;
			}
			return path;

		}

		private string GetPathString(UCommerce.EntitiesV2.Category category) {

			string path = category.Name;
			if (category.ParentCategory != null)
			{
				path = GetPathString(category.ParentCategory) + "/" + path;
			}
			else
			{
				path = category.ProductCatalog.Name + "/" + path;
			}
			return path;

		}

        protected void clearProductSelection_Clicked(object sender, EventArgs e) {
            _umbracoValue = "";
            treeProduct.CollapseAll();
            treeProduct.Nodes.Clear();

            InitTree();
            selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/error.png";
            selectedProductLink.Text = "";
            clearProductSelection.Visible = false;
        }

    }

}