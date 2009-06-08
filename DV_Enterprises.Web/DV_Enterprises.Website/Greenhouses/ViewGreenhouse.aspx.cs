using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using DV_Enterprises.Web.Data.Domain;
using DV_Enterprises.Web.Data.Filters;
using DV_Enterprises.Web.Service;
using DV_Enterprises.Web.Service.Interface;

namespace Greenhouses
{
    public partial class ViewGreenhouse : Page
    {
        private readonly static IWebContext WebContext = new WebContext();
        private readonly static IRedirector Redirector = new Redirector();

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlUsers.Visible = User.IsInRole("administrator");
            if (IsPostBack) return;
            if (WebContext.GreenhouseId <= 0) Redirector.GoToGreenhouses();
            WebContext.GreenhouseIdSession = WebContext.GreenhouseId;
            Bind();
        }

        #region Load Data

        private void Bind()
        {
            var greenhouse = Greenhouse.Find().ByID(WebContext.GreenhouseIdSession);
            LoadData(greenhouse);
            LoadLocation(greenhouse.Address);
            LoadSection(greenhouse.Sections.ToList());
            LoadGreenhouseUsers(greenhouse.GreenhouseUsers);
        }

        private void LoadGreenhouseUsers(IEnumerable<GreenhouseUser> users)
        {
            var userList = new List<MembershipUser>();
            foreach (MembershipUser user in Membership.GetAllUsers())
            {
                if (!users.Where(u => u.Username == user.UserName).Any()) { userList.Add(user); }
            }

            if (userList.Any())
            {
                ddlUsers.Visible = true;
                butAddUsers.Visible = true;
                ddlUsers.DataSource = userList;
                ddlUsers.DataTextField = "UserName";
                ddlUsers.DataValueField = "ProviderUserKey";
                ddlUsers.DataBind();
            }
            else
            {
                ddlUsers.Visible = false;
                butAddUsers.Visible = false;
            }

            lvUsers.DataSource = users;
            lvUsers.DataBind();
        }

        public void LoadData(Greenhouse greenhouse)
        {
            Page.Title = string.Format("{0} &mdash; Smart Greenhouse Solutions", greenhouse);
            litGreenhouseTitle.Text = greenhouse.ToString();
        }

        public void LoadLocation(Address address)
        {
            tbxSteetAddress1.Text = litStreetAddress1.Text = address.StreetLine1;
            if (!string.IsNullOrEmpty(address.StreetLine2))
            {
                litStreetAddress2.Visible = true;
                litStreetAddress2BR.Visible = true;
                tbxSteetAddress2.Text = litStreetAddress2.Text = address.StreetLine2;
            }
            else
            {
                litStreetAddress2.Visible = false;
                litStreetAddress2BR.Visible = false;
            }
            tbxCity.Text = litCity.Text = address.City;
            tbxState.Text = litState.Text = address.StateOrProvince;
            tbxCountry.Text = litCountry.Text = address.Country;
            tbxZipCode.Text = litZip.Text = address.Zip.ToString();
            cboIsDefault.Checked = address.IsDefault;
        }

        public void LoadSection(List<Section> sections)
        {
            // If the user is an administrator return all of the Greenhouse's  sections if not
            // then only return the ones that they are the owners of.
            sections = User.IsInRole("administrator")
                           ? sections
                           : sections.Where(s => s.Username == User.Identity.Name).ToList();
            lvSections.DataSource = sections;
            lvSections.DataBind();
        }

        #endregion

        #region User ListView

        protected void lvUsers_ItemDeleting(object sender, ListViewDeleteEventArgs e) { }
        protected void lvUsers_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Delete":
                    // remove user from greenhouse
                    DeleteGreenhouseUser(e.Item);
                    break;
            }
        }

        private void DeleteGreenhouseUser(ListViewItem item)
        {
            var g = GreenhouseUser.Find().Where(gu => gu.GreenhouseID == WebContext.GreenhouseIdSession && gu.Username == ((Literal)item.FindControl("litUsername")).Text).SingleOrDefault();
            g.Delete();
            Bind();
        }

        #endregion

        #region Sections ListView

        public void lvSections_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var litPresetID = e.Item.FindControl("litPresetID") as Literal;
            var ddlPreset = e.Item.FindControl("ddlPreset") as DropDownList;
            if (ddlPreset != null)
            {
                ddlPreset.DataSource = Preset.Find();
                ddlPreset.DataTextField = "Name";
                ddlPreset.DataValueField = "ID";
                if (litPresetID != null) ddlPreset.SelectedValue = litPresetID.Text;
                ddlPreset.DataBind();
            }

            var pnlOwner = e.Item.FindControl("pnlOwner") as Panel;
            if (pnlOwner != null)
            {
                pnlOwner.Visible = User.IsInRole("administrator");
            }

            var litUserID = e.Item.FindControl("litUserID") as Literal;
            var ddlOwner = e.Item.FindControl("ddlOwner") as DropDownList;
            if (ddlOwner != null)
            {
                var users = Greenhouse.Find().ByID(WebContext.GreenhouseIdSession).GreenhouseUsers;
                ddlOwner.DataSource = users;
                ddlOwner.DataTextField = "Username";
                ddlOwner.DataValueField = "UserID";
                if (litUserID != null) ddlOwner.SelectedValue = litUserID.Text;
                ddlOwner.DataBind();
            }

            var litIsTemperatureActivated = e.Item.FindControl("litIsTemperatureActivated") as Literal;
            var pnlTemperature = e.Item.FindControl("pnlTemperature") as Panel;
            if (litIsTemperatureActivated != null)
            {
                if (pnlTemperature != null) pnlTemperature.Visible = bool.Parse(litIsTemperatureActivated.Text);
            }

            var litIsLightActivated = e.Item.FindControl("litIsLightActivated") as Literal;
            var pnlLighting = e.Item.FindControl("pnlLighting") as Panel;
            if (litIsLightActivated != null)
            {
                if (pnlLighting != null) pnlLighting.Visible = bool.Parse(litIsLightActivated.Text);
            }

            var litIsHumidityActivated = e.Item.FindControl("litIsHumidityActivated") as Literal;
            var pnlHumidity = e.Item.FindControl("pnlHumidity") as Panel;
            if (litIsHumidityActivated != null)
            {
                if (pnlHumidity != null) pnlHumidity.Visible = bool.Parse(litIsHumidityActivated.Text);
            }

            var litIsWaterLevelActivated = e.Item.FindControl("litIsWaterLevelActivated") as Literal;
            var pnlWaterLevel = e.Item.FindControl("pnlWaterLevel") as Panel;
            if (litIsWaterLevelActivated != null)
            {
                if (pnlWaterLevel != null) pnlWaterLevel.Visible = bool.Parse(litIsWaterLevelActivated.Text);
            }

            var lblNoModules = e.Item.FindControl("lblNoModules") as Label;
            if (litIsTemperatureActivated != null
                && litIsLightActivated != null
                && litIsHumidityActivated != null
                && litIsWaterLevelActivated != null)
            {
                if (!bool.Parse(litIsTemperatureActivated.Text)
                    && !bool.Parse(litIsLightActivated.Text)
                    && !bool.Parse(litIsHumidityActivated.Text)
                    && !bool.Parse(litIsWaterLevelActivated.Text))
                {
                    if (lblNoModules != null) lblNoModules.Visible = true;
                }
            }
        }

        protected void lvSections_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            CloseInsert();
            lvSections.EditIndex = e.NewEditIndex;
            Bind();
        }

        protected void lvSections_ItemInserting(object sender, ListViewInsertEventArgs e) { }
        protected void lvSections_ItemUpdating(object sender, ListViewUpdateEventArgs e) { }
        protected void lvSections_ItemDeleting(object sender, ListViewDeleteEventArgs e) { }
        protected void lvSections_SelectedIndexChanged(object sender, EventArgs e) { }

        protected void lvSections_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            if (e.CancelMode == ListViewCancelMode.CancelingInsert)
            {
                CloseInsert();
            }
            else
            {
                lvSections.EditIndex = -1;
            }
            //Bind();
            Redirector.GoToViewGreenhouse(WebContext.GreenhouseIdSession);
        }

        protected void lvSections_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "Insert":
                    // Save new section
                    InsertSection(e.Item);
                    break;
                case "Update":
                    // update editied section
                    UpdateSection(e.Item);
                    break;
                case "Delete":
                    // delete old section
                    DeleteSection(e.Item);
                    break;
            }
        }

        protected void lbNewSection_Click(object sender, EventArgs e)
        {

            lvSections.EditIndex = -1;
            lvSections.InsertItemPosition = InsertItemPosition.FirstItem;
            ((LinkButton)sender).Visible = false;
            Bind();

            var ddlPreset = lvSections.InsertItem.FindControl("ddlPreset") as DropDownList;

            if (ddlPreset == null) return;
            ddlPreset.DataSource = Preset.Find();
            ddlPreset.DataTextField = "Name";
            ddlPreset.DataValueField = "ID";
            ddlPreset.DataBind();

            var pnlOwner = lvSections.InsertItem.FindControl("pnlOwner") as Panel;
            if (pnlOwner != null)
            {
                pnlOwner.Visible = User.IsInRole("administrator");
            }

            var ddlOwner = lvSections.InsertItem.FindControl("ddlOwner") as DropDownList;
            if (ddlOwner != null)
            {
                var users = Greenhouse.Find().ByID(WebContext.GreenhouseIdSession).GreenhouseUsers;
                ddlOwner.DataSource = users;
                ddlOwner.DataTextField = "Username";
                ddlOwner.DataValueField = "UserID";
                ddlOwner.DataBind();
            }
        }

        private void CloseInsert()
        {
            lvSections.InsertItemPosition = InsertItemPosition.None;
            lbNewSection.Visible = true;
        }

        private void UpdateSection(Control item)
        {
            var ddlOwner = ((DropDownList)item.FindControl("ddlOwner")).SelectedValue;
            var litUserID = ((Literal)item.FindControl("litUserID")).Text;
            var userID = User.IsInRole("administrator") ? ddlOwner : litUserID;
            new Section
                {
                    ID = Convert.ToInt32(((Literal)item.FindControl("litSectionID")).Text),
                    Name = ((TextBox)item.FindControl("tbxName")).Text,
                    GreenhouseID = WebContext.GreenhouseIdSession,
                    UserID = new Guid(userID),
                    PresetID = Convert.ToInt32(((DropDownList)item.FindControl("ddlPreset")).SelectedValue),
                    IsTemperatureActivated = ((CheckBox)item.FindControl("cboIsTemperatureActivated")).Checked,
                    IdealTemperature = ((TextBox)item.FindControl("tbxIdealTemperature")).Text.ToNullableInt(),
                    TemperatureThreshold = ((TextBox)item.FindControl("tbxTemperatureTreshold")).Text.ToNullableInt(),
                    IsLightActivated = ((CheckBox)item.FindControl("cboIsLightActivated")).Checked,
                    IdealLightIntensity = ((TextBox)item.FindControl("tbxIdealLightIntensity")).Text.ToNullableInt(),
                    LightIntensityThreshold = ((TextBox)item.FindControl("tbxLightIntensityTreshold")).Text.ToNullableInt(),
                    IsHumidityActivated = ((CheckBox)item.FindControl("cboIsHumidityActivated")).Checked,
                    IdealHumidity = ((TextBox)item.FindControl("tbxIdealHumidity")).Text.ToNullableInt(),
                    HumidityThreshold = ((TextBox)item.FindControl("tbxHumidityTreshold")).Text.ToNullableInt(),
                    IsWaterLevelActivated = ((CheckBox)item.FindControl("cboIsWaterLevelActivated")).Checked,
                    IdealWaterLevel = ((TextBox)item.FindControl("tbxIdealWaterLevel")).Text.ToNullableInt(),
                    WaterLevelThreshold = ((TextBox)item.FindControl("tbxWaterLevelThreshold")).Text.ToNullableInt()
                }.Save();
            lvSections.EditIndex = -1;
            //Bind();
            Redirector.GoToViewGreenhouse(WebContext.GreenhouseIdSession);
        }


        /// <summary>
        /// inserts a new section. At the moment it only inserts the current User as the section owner
        /// </summary>
        /// <param name="item">ListView item. This should be a section</param>
        private void InsertSection(Control item)
        {
            var ddlOwner = ((DropDownList)item.FindControl("ddlOwner")).SelectedValue;
            var litUserID = Membership.GetUser().ProviderUserKey.ToString();
            var userID = User.IsInRole("administrator") ? ddlOwner : litUserID;
            new Section
                {
                    ID = 0,
                    Name = ((TextBox)item.FindControl("tbxName")).Text,
                    GreenhouseID = WebContext.GreenhouseIdSession,
                    UserID = new Guid(userID),
                    PresetID = Convert.ToInt32(((DropDownList)item.FindControl("ddlPreset")).SelectedValue),
                    IsTemperatureActivated = ((CheckBox)item.FindControl("cboIsTemperatureActivated")).Checked,
                    IdealTemperature = ((TextBox)item.FindControl("tbxIdealTemperature")).Text.ToNullableInt(),
                    TemperatureThreshold = ((TextBox)item.FindControl("tbxTemperatureTreshold")).Text.ToNullableInt(),
                    IsLightActivated = ((CheckBox)item.FindControl("cboIsLightActivated")).Checked,
                    IdealLightIntensity = ((TextBox)item.FindControl("tbxIdealLightIntensity")).Text.ToNullableInt(),
                    LightIntensityThreshold = ((TextBox)item.FindControl("tbxLightIntensityTreshold")).Text.ToNullableInt(),
                    IsHumidityActivated = ((CheckBox)item.FindControl("cboIsHumidityActivated")).Checked,
                    IdealHumidity = ((TextBox)item.FindControl("tbxIdealHumidity")).Text.ToNullableInt(),
                    HumidityThreshold = ((TextBox)item.FindControl("tbxHumidityTreshold")).Text.ToNullableInt(),
                    IsWaterLevelActivated = ((CheckBox)item.FindControl("cboIsWaterLevelActivated")).Checked,
                    IdealWaterLevel = ((TextBox)item.FindControl("tbxIdealWaterLevel")).Text.ToNullableInt(),
                    WaterLevelThreshold = ((TextBox)item.FindControl("tbxWaterLevelThreshold")).Text.ToNullableInt()
                }.Save();
            CloseInsert();
            //Bind();
            Redirector.GoToViewGreenhouse(WebContext.GreenhouseIdSession);
        }

        private void DeleteSection(Control item)
        {
            Section.All().ByID(Convert.ToInt32(((Literal)item.FindControl("litSectionID")).Text)).Delete();
            Bind();
        }

        #endregion

        public string PresetValue(int? s)
        {
            return s == null ? string.Empty : string.Format("<span class='section_preset'>{0}</span>", s.ToString());
        }

        protected void linkEdit_Click(object sender, EventArgs e)
        {
            linkEdit.Visible = false;
            pnlEditGreenhouse.Visible = true;
            pnlGreenhouseAddress.Visible = false;
        }

        protected void butUpdateGreenhouse_Click(object sender, EventArgs e)
        {
            var g = new Greenhouse
                        {
                            ID = WebContext.GreenhouseIdSession,
                            Address = new Address
                                          {
                                              City = tbxCity.Text,
                                              Country = tbxCountry.Text,
                                              IsDefault = cboIsDefault.Checked,
                                              StateOrProvince = tbxState.Text,
                                              StreetLine1 = tbxSteetAddress1.Text,
                                              StreetLine2 = tbxSteetAddress2.Text,
                                              Zip = tbxZipCode.Text.ToNullableInt()
                                          }
                        };
            g.Save();
            LoadLocation(g.Address);
            linkEdit.Visible = true;
            pnlEditGreenhouse.Visible = false;
            pnlGreenhouseAddress.Visible = true;
        }

        protected void linkCancelUpdateGreenhouse_Click(object sender, EventArgs e)
        {
            linkEdit.Visible = true;
            pnlEditGreenhouse.Visible = false;
            pnlGreenhouseAddress.Visible = true;
        }

        protected void butAddUsers_Click(object sender, EventArgs e)
        {
            var userid = new Guid(ddlUsers.SelectedValue);
            if (GreenhouseUser.Find().Where(gu => gu.UserID == userid).SingleOrDefault() == null)
            {
                new GreenhouseUser
                    {
                        UserID = userid,
                        GreenhouseID = WebContext.GreenhouseIdSession,
                    }.Save();
            }
            Bind();
        }
    }
}