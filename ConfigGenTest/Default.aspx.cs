using System;
using System.Collections.Generic;
using System.Web.UI;

namespace ConfigGenTest
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string Environment = System.Configuration.ConfigurationManager.AppSettings["Environment"];

            Response.Write(Environment);
        }
    }
}