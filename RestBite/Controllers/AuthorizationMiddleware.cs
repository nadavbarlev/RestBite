using RestBite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestBite.Controllers
{
    public static class AuthorizationMiddleware
    {
        public static bool AdminAuthorized(HttpSessionStateBase Session)
        {
            return (((Client)Session["Client"]) != null && ((Client)Session["Client"]).isAdmin);
        }

        public static bool Authorized(HttpSessionStateBase Session)
        {
            return (((Client)Session["Client"]) != null);
        }
    }   
}