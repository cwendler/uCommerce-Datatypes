using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace byte5.UCommerceExt.Datatypes {

    public partial class Country : System.Web.UI.UserControl, umbraco.editorControls.userControlGrapper.IUsercontrolDataEditor {

        private string _umbracoValue;

		protected void Page_Load(object sender, EventArgs e) {

			if(!IsPostBack) {
				var countries = UCommerce.EntitiesV2.Country.All();
				ddlCountry.DataSource = countries;
				ddlCountry.DataValueField = "CountryId";
				ddlCountry.DataTextField = "Name";
				ddlCountry.DataBind();
				try {
					//Response.Write("Setting Country Checkbox to " + _umbracoValue + "<br/>");
					ddlCountry.SelectedValue = _umbracoValue;
					//Response.Write("CountryName: " + ddlCountry.SelectedItem.Text + "<br/>");
				}
				catch(Exception ex) {
					Response.Write("Something went wrong: " + ex.Message + "<br/>");
					_umbracoValue = "";
				}
			}
        }

        public object value {
            get {
                return ddlCountry.SelectedValue;
            }
            set {
                if (value != null) {
                    _umbracoValue = value.ToString();
                }
            }
        }

    }

}