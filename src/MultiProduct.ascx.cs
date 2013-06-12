using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace byte5.UCommerceExt.Datatypes {

	public partial class MultiProduct : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor {

		/// <summary>
		/// The Value that is saved and loaded by the Usercontrol
		/// </summary>
		private string _umbracoValue;
		/// <summary>
		///  A list of the selected Product SKUs
		/// </summary>
		private List<string> selectedSKUs;
		/// <summary>
		/// A List of the selected Product Names
		/// </summary>
 		private List<string> selectedNodeNames = new List<string>();

		private string catalogPrefix = "catalog";
		private string categoryPrefix = "category";
		private string productPrefix = "product";

		/// <summary>
		/// The standard Page_Load function. Builds the Product-Tree, if it is no Postback.Also Sets the label text.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e) {

            treeMultiProduct.Attributes.Add("onclick", "postBackByObjectMulti()");
			if(!IsPostBack) {
                clearMultiSelection.ImageUrl = "/umbraco/images/close.png";
			    InitTree();
			}
			else {
				_umbracoValue = (string)Session[this.GetType().ToString() + "_" + this.UniqueID + "_umbracoValue_string"];
				SetSelectedSKUs();
				CreateSelectedNamesList();
				SetProductLabelText();
			}

		}

        private void InitTree() {

            SetSelectedSKUs();
            CreateSelectedNamesList();
            var catalogs = from catalog in UCommerce.EntitiesV2.ProductCatalog.All() select catalog;

            // Anfangen den TreeView aufzubauen. Die obersten Knoten bilden die Kataloge
            foreach(UCommerce.EntitiesV2.ProductCatalog productCatalog in catalogs) {
                TreeNode tn = new TreeNode(productCatalog.Name, catalogPrefix + "|" + productCatalog.ProductCatalogId.ToString());
                tn.ImageUrl = "/umbraco/images/umbraco/folder.gif";
                //Den Knoten werden dann die obersten Kategorien als Unterknoten hinzu gefügt
                tn.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
                if(treeMultiProduct != null) {
                    treeMultiProduct.Nodes.Add(tn);
                }
            }

            Session[this.GetType().ToString() + "_" + this.UniqueID + "_umbracoValue_string"] = GetSelectedProducts();
            Session[this.GetType().ToString() + "_" + this.UniqueID + "_productNames_string"] = GetSelectedNames();

            SetProductLabelText();

        }

		private void CreateSelectedNamesList() {

			selectedNodeNames = new List<string>();
			foreach (var sku in selectedSKUs) {
				var product =
					UCommerce.EntitiesV2.Product.All().SingleOrDefault(
						p => p.Sku == sku && (p.VariantSku == null || p.VariantSku == ""));
				if (product != null)
					selectedNodeNames.Add(product.Name);
			}

		}

		/// <summary>
		/// Fills the selectedSKUs variable by splitting the _umbracoValue string into a list. The character '|' is used as the separator.
		/// </summary>
		private void SetSelectedSKUs() {

			if(!string.IsNullOrEmpty(_umbracoValue)) {
				try {
					selectedSKUs = _umbracoValue.Split(new char[] { '|' }).ToList();
				}
				catch(Exception ex) {
					Response.Write(ex.Message + ", _umbracoValue = " + _umbracoValue + "<br/>");
				}
			}
			else {
				selectedSKUs = new List<string>();
			}

		}

		/// <summary>
		/// The Datatype Usercontrol Wrapper function, that loads and saves the value set by the Usercontrol.
		/// </summary>
		public object value {
			get {
				string products = GetSelectedProducts();
				if(products != null)
					return products;
				return null;
			}
			set {
				if(value != null) {
					_umbracoValue = value.ToString();
				}
				else {
					_umbracoValue = "";
				}
			}
		}

		/// <summary>
		/// Iterates through the selectedSKUs variable and concetenates the product skus. The skus are seperated by a '|' character.
		/// </summary>
		/// <returns>The product skus in a single string, separated by a '|' character.</returns>
		protected string GetSelectedProducts() {

			string products = "";

			if (selectedSKUs != null)
			{
				for(int i = 0; i < selectedSKUs.Count; i++) {
					string product = selectedSKUs[i];
					if(!String.IsNullOrEmpty(product)) {
						products += product;
						if(i < selectedSKUs.Count - 1) {
							products += "|";
						}
					}
				}
			}
			return products;

		}

		/// <summary>
		/// Iterates through the selectedNodeNames variable and concetenates the product names. The names are seperated by a '|' character.
		/// </summary>
		/// <returns>The product names in a single string, separated by a '|' character.</returns>
		protected string GetSelectedNames() {

			string names = "";

			if (selectedNodeNames != null)
			{
				for (int i = 0; i < selectedNodeNames.Count; i++)
				{
					string name = selectedNodeNames[i];
					if(!String.IsNullOrEmpty(name))
					{
						names += name;
						if (i < selectedNodeNames.Count - 1)
						{
							names += "|";
						}
					}
				}
			}
			return names;

		}

		/// <summary>
		/// The Event, that is fired, when a checkbox is changed. Is used to add and remove product skus and names from the corresponding lists.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void treeMultiProduct_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e) {

			// The concetenated SKU string is restored from the Session variable
			_umbracoValue = (string)Session[this.GetType().ToString() + "_" + this.UniqueID + "_umbracoValue_string"];
			SetSelectedSKUs();
			CreateSelectedNamesList();

			TreeNode checkedNode = e.Node;
			string[] val = checkedNode.Value.Split('|');
			// Checks the product nodes, whose checkboxes were changed. Updates the SKU and Names list accordingly.
			if(val[0] == productPrefix) {
				if(checkedNode.Checked) {
					if (!selectedSKUs.Contains(val[1])) {
						selectedSKUs.Add(val[1]);
						if (!selectedNodeNames.Contains(checkedNode.Text))
							selectedNodeNames.Add(checkedNode.Text);
					}
				}
				else {
					selectedSKUs.Remove(val[1]);
					selectedNodeNames.Remove(checkedNode.Text);
				}
				
			}
			Session[this.GetType().ToString() + "_" + this.UniqueID + "_umbracoValue_string"] = GetSelectedProducts();
			Session[this.GetType().ToString() + "_" + this.UniqueID + "_productNames_string"] = GetSelectedNames();

			SetProductLabelText();
            //MultiProductPanel.Update();
		}

		protected void treeMultiProduct_NodeExpanded(object sender, TreeNodeEventArgs tnea) {

			_umbracoValue = (string)Session[this.GetType().ToString() + "_" + this.UniqueID + "_umbracoValue_string"];
			SetSelectedSKUs();
			CreateSelectedNamesList();

			TreeNode n = tnea.Node;
			if(n.ChildNodes[0].Value == "NULL_NODE") {
				n.ChildNodes.RemoveAt(0);
				string[] val = n.Value.Split('|');
				if(val[0] == catalogPrefix) {
					int catalogId = int.Parse(val[1]);
					var catalog = UCommerce.EntitiesV2.ProductCatalog.All().SingleOrDefault(cata => cata.ProductCatalogId == catalogId);
					if(catalog != null) {
						foreach(var category in catalog.Categories.Where(cate => cate.ParentCategory == null)) {
							TreeNode catNode = new TreeNode(category.Name, categoryPrefix + "|" + category.CategoryId.ToString()) { ImageUrl = "/umbraco/images/umbraco/folder.gif" };
							if(category.Categories.Count > 0 || category.Products.Count() > 0) {
								catNode.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
							}
							n.ChildNodes.Add(catNode);
						}
					}
				}
				else if(val[0] == categoryPrefix) {
					int categoryId = int.Parse(val[1]);
					var category = UCommerce.EntitiesV2.Category.All().SingleOrDefault(cate => cate.CategoryId == categoryId);
					if(category != null) {
						foreach(var subCat in category.Categories) {
							TreeNode catNode = new TreeNode(subCat.Name, categoryPrefix + "|" + subCat.CategoryId.ToString()) { ImageUrl = "/umbraco/images/umbraco/folder.gif" };
							if(subCat.Categories.Count > 0 || subCat.Products.Count() > 0) {
								catNode.ChildNodes.Add(new TreeNode("", "NULL_NODE"));
							}
							n.ChildNodes.Add(catNode);
						}
						foreach(var product in category.Products) {
							TreeNode prodNode = new TreeNode(product.Name, productPrefix + "|" + product.Sku) { ImageUrl = "/umbraco/images/umbraco/package.gif" };
							prodNode.ShowCheckBox = true;
							n.ChildNodes.Add(prodNode);
							if(selectedSKUs != null && selectedSKUs.Contains(product.Sku)) {
								prodNode.Checked = true;
							}
						}

					}
				}
			}

		}

		/// <summary>
		/// Sets the label to show the selected products.
		/// </summary>
		private void SetProductLabelText() {

			if(selectedNodeNames.Count > 0) {
			    selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/success.png";
				selectedProductLink.Text = "";
				for(int i = 0; i < selectedNodeNames.Count; i++) {
					if(!String.IsNullOrEmpty(selectedNodeNames[i])) {
						selectedProductLink.Text += selectedNodeNames[i];
						if(i < selectedNodeNames.Count - 1) {
							selectedProductLink.Text += "<br/>";
						}
					}
				}
			    clearMultiSelection.Visible = true;
			}
			else {
                selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/error.png";
				selectedProductLink.Text = "";
			    clearMultiSelection.Visible = false;
			}
			
		}

		/// <summary>
		/// Reads the selected names from a concatenated string and stores them in the selectedNodeNames List. The string is seperated at the character '|'.
		/// </summary>
		/// <param name="selectedNamesString">The concatenated Name string</param>
		private void CreateSelectedNamesList(string selectedNamesString) {

			if(!string.IsNullOrEmpty(selectedNamesString)) {
				try {
					selectedNodeNames = selectedNamesString.Split(new char[] { '|' }).ToList();
				}
				catch(Exception ex) {
					Response.Write(ex.Message + ", selectedNamesString = " + selectedNamesString + "<br/>");
				}
			}
			else {
				selectedNodeNames = new List<string>();
			}
			
		}

        protected void clearMultiSelection_Clicked(object sender, EventArgs e) {
            _umbracoValue = "";
            selectedNodeNames.Clear();
            selectedSKUs.Clear();
            treeMultiProduct.CollapseAll();
            treeMultiProduct.Nodes.Clear();
            selectedProductIcon.ImageUrl = "/umbraco/images/speechBubble/error.png";
            InitTree();
        }

	}
}