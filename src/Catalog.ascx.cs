using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace byte5.UCommerceExt.Datatypes {

    public partial class Catalog : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor {

        private string _umbracoValue;

        protected void Page_Load(object sender, EventArgs e) {
			if(!IsPostBack) {
				var catalogs = from catalog in UCommerce.EntitiesV2.ProductCatalog.All() select catalog.Name;
				ddlCatalog.DataSource = catalogs;
				ddlCatalog.DataBind();
				try {
					//Response.Write("Setting Selected Catalog to " + _umbracoValue + "<br/>");
					ddlCatalog.SelectedValue = _umbracoValue;
				}
				catch(Exception ex) {
					Response.Write("Something went wrong! " + ex.Message + "<br/>");
					_umbracoValue = "";
				}
			}
        }

        public object value {
            get {
				//Response.Write("Catalog.object: Sending Value " + ddlCatalog.SelectedValue + "<br />");
                return ddlCatalog.SelectedValue;
            }
            set {
                if (value != null) {
                    _umbracoValue = value.ToString();
					//Response.Write("Catalog.object: Receiving Value " + _umbracoValue + "<br />");
				}
            }
        }

    }

}