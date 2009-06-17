using System;
using System.Web.UI;

public partial class Default : Page
{
    protected void Page_Load(object sender, EventArgs e){}

    protected void lbMeanGreens_Click(object sender, EventArgs e)
    {
        Response.Redirect("MeanGreens.aspx");
    }
}
