using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Configuration;

namespace Idla.PtaProxy.Admin {
    public partial class Register : System.Web.UI.Page {
        int savedSelectedIndex = -1;
        protected void Page_Load(object sender, EventArgs e) {

        }

        protected void btnRegister_Click(object sender, EventArgs e) {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".btnRegister_Click(): ");
            try {
                String bb_code = ddlBBoard.SelectedValue;
                //DataValueField.
                String bbRegPwd = txtBBRegPwd.Text;
                //String  proxy_folder = Request.ApplicationPath + Request.Path;
                String proxy_folder = Request.Path;
                proxy_folder = proxy_folder.ToLower().Replace("/admin/register.aspx", "");
                //remove prefixed "/" 
//                if (proxy_folder.Length > 0) proxy_folder = proxy_folder.Substring(1);
                if (proxy_folder.Length != 0) {
                    if (proxy_folder.Substring(0, 1) != "/") proxy_folder = "/" + proxy_folder;
                    if (proxy_folder.Substring(proxy_folder.Length - 1, 1) != "/") proxy_folder += "/";
                } else proxy_folder = "/";
                log.Debug("Request.ApplicationPath: " + Request.ApplicationPath + "; Request.Path: " + Request.Path + "; proxy_folder: " + proxy_folder);
                //if (proxy_folder.Length != 0) proxy_folder = proxy_folder.Substring(1); 
                //                if (!PtaUtil.GetProxyWebFolder().Contains(proxy_folder))
//                if (!PtaUtil.GetProxyWebFolder().Equals(proxy_folder))
//                    throw new Idla.PtaProxy.PtaException("Possibly bad AppSettings[\"PtaProxyWebFolder\"] = " + WebConfigurationManager.AppSettings["PtaProxyWebFolder"]
//                            + "; Calculated proxy_folder = " + proxy_folder + "; PtaUtil.GetProxyWebFolder(): " + PtaUtil.GetProxyWebFolder());
                Idla.PtaProxy.WSCall.RegisterTool(bb_code, bbRegPwd, false);
                lblRegisterResult.Text = "SUCCESS";
                if (!PtaUtil.GetProxyWebFolder().Equals(proxy_folder)) {
                    lblRegisterResult.Text = lblRegisterResult.Text + ", but possibly bad AppSettings[\"PtaProxyWebFolder\"] = " + WebConfigurationManager.AppSettings["PtaProxyWebFolder"]
                            + "; Calculated proxy_folder = " + proxy_folder + "; PtaUtil.GetProxyWebFolder(): " + PtaUtil.GetProxyWebFolder() + ". Check it if link redirection or BB actions will fail";
                }
                gvBBoard.DataBind();
            } catch (PtaRegisterToolException rte) {
                lblRegisterResult.Text = "ERROR: " + rte.Message;
            }
        }

        protected void btnPreviewRegXml_Click(object sender, EventArgs e) {
            String bb_code = ddlBBoard.SelectedValue;;
            String xml = Idla.PtaProxy.RegisterToolWSCall.GetRegistrationXML(bb_code);
            txtRegXml.Text = xml;
        }

        protected void gvBBoard_SelectedIndexChanged(object sender, EventArgs e) {
            String bb_code = gvBBoard.SelectedValue.ToString();
            txtXML.Text = PtaUtil.GetBBMenuLinkParamXML(bb_code);
            lblSaveXMLResult.Text = "";
        }

        protected void btnBBoardAdd_Click(object sender, EventArgs e) {
            dsBBoard.Insert();
        }

        protected void btnSave_Click(object sender, EventArgs e) {
//            try {
                PtaUtil.SetBBMenuLinkParamXML(gvBBoard.SelectedValue.ToString(), txtXML.Text);
                lblSaveXMLResult.Text = "SUCCESS";
//            } catch (PtaRegisterToolException rte) {
//                lblRegisterResult.Text = "ERROR: " + rte.Message;
//            }
        }

        protected void ddlBBoard_SelectedIndexChanged(object sender, EventArgs e) {
            lblRegisterResult.Text = "";
        }

        protected void dsBBoard_Updating(object sender, SqlDataSourceCommandEventArgs e) {
            //
            //dsBBoard.UpdateParameters["OldBBoardURL"].DefaultValue = (string)gvBBoard.SelectedDataKey["BBoardURL"];
        }

        protected void gvBBoard_RowUpdating(object sender, GridViewUpdateEventArgs e) {
            savedSelectedIndex = e.RowIndex;
            //different value of SelectedIndex caused NULL fields to obtain key value (BBoardURL) of currently selected record,
            //which could be different from updating one (edit does not cause selection to change)
            gvBBoard.SelectedIndex = -1;
            String old_bb_code = (string)e.Keys["BBoardCode"];
            //BBoardListValue
            dsBBoard.UpdateParameters["OldBBoardCode"].DefaultValue = old_bb_code;
            String new_bb_code = (string)e.NewValues["BBoardCode"];
            dsBBoard.UpdateParameters["NewBBoardCode"].DefaultValue = new_bb_code;
        }

        protected void gvBBoard_RowUpdated(object sender, GridViewUpdatedEventArgs e) {
            gvBBoard.SelectedIndex = savedSelectedIndex;
        }

        protected void btnRefresh_Click(object sender, EventArgs e) {
            gvBBoard.DataBind();
        }

        protected void btnRefreshList_Click(object sender, EventArgs e) {
            ddlBBoard.DataBind();
        }

        protected void btnSetPassword_Click(object sender, EventArgs e) {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".btnSetPassword_Click(): ");
            MembershipUser oldUser = Membership.Provider.GetUser("pta_admin", false);
            if (oldUser != null) {
                bool res;
                res = Membership.Provider.ChangePassword("pta_admin", txtPtaOldPwd.Text, txtPtaNewPwd.Text);
                if (res) lblSetPassword.Text = "SUCCESS";
                else lblSetPassword.Text = "ERROR";
            } else lblSetPassword.Text = "pta_admin does not exist, create it first";
            /*
            if (oldUser != null) {
                bool res;
                res = Membership.Provider.ChangePassword("pta_admin", txtPtaOldPwd.Text, txtPtaNewPwd.Text);
                if (res) lblSetPassword.Text = "SUCCESS";
                else lblSetPassword.Text = "ERROR";
            } else {
                MembershipCreateStatus status;
                MembershipUser newUser =
                            Membership.Provider.CreateUser("pta_admin", txtPtaNewPwd.Text, string.Empty, string.Empty, string.Empty, true, "admin", out status);
                if (newUser == null) {
                    lblCreatePtaAdmin.Text = status.ToString();
                } else lblCreatePtaAdmin.Text = "SUCCESS";
            }*/
        }

        protected void btnCreatePtaAdmin_Click(object sender, EventArgs e) {
            log4net.ILog log = PtaUtil.getLog4netLogger(this.GetType().FullName + ".btnCreatePtaAdmin_Click(): ");
            MembershipUser oldUser = Membership.Provider.GetUser("pta_admin", false);
            if (oldUser == null) {
                MembershipCreateStatus status;
                MembershipUser newUser =
                            Membership.Provider.CreateUser("pta_admin", txtPtaNewPwd.Text, string.Empty, string.Empty, string.Empty, true, "admin", out status);
                if (newUser == null) {
                    lblCreatePtaAdmin.Text = status.ToString();
                } else lblCreatePtaAdmin.Text = "SUCCESS";
            } else lblCreatePtaAdmin.Text = "pta_admin user already exists, it may be deleted by manual editing of pta_proxy\\App_Data\\users.xml and restarting of application.";



        }
    }
}
