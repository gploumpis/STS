<%@ Application Language="C#" %>

<script runat="server">

   
    void Application_Start(object sender, EventArgs e) 
    {
        //var path = System.Web.Hosting.HostingEnvironment.MapPath("~/issuer.model.local.cer");
        //STS.Core.JWTSecurityTokenHandlerFix.AddIssuerKey(new System.Security.Cryptography.X509Certificates.X509Certificate2(path));
    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }


    void Application_BeginRequest(object sender, EventArgs e)
    {
       
        if (HttpContext.Current.Request.HttpMethod =="POST" )
        {
            HttpContext.Current.Response.Status = "302 Found";
            HttpContext.Current.Response.StatusCode = 302;
            HttpContext.Current.Response.AddHeader("Location", HttpContext.Current.Request.Url.ToString());
        }  
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
