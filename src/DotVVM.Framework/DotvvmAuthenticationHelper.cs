using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using DotVVM.Framework.Hosting;

namespace DotVVM.Framework
{
    public class DotvvmAuthenticationHelper
    {
        /// <summary>
        /// Fixes the response created by the OWIN Security Challenge call to be accepted by DotVVM client library.
        /// </summary>
        public static void ApplyRedirectResponse(IOwinContext context, string redirectUri)
        {
            if (context.Response.StatusCode == 401)
            {
                DotvvmRequestContext.SetRedirectResponse(context, redirectUri, 200);
            }
        }
    }
}