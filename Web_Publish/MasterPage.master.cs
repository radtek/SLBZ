using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    internal string GetAspTitle()
    {
        string title = "";

        String serverName = Server.MachineName;

        if (serverName.Equals("GZ-20160416DNOK", StringComparison.CurrentCultureIgnoreCase))
        {
            title = "Win7";
        }
        else if (serverName.Equals("EV08382-01", StringComparison.CurrentCultureIgnoreCase))
        {
            title = "VLF";
        }
        else if (serverName.Equals("EVOAOGUANG", StringComparison.CurrentCultureIgnoreCase))
        {
            title = "800";
        }
        else
        {
            title = "null";
        }

        return title + "_出版记录";
    }

   
}
