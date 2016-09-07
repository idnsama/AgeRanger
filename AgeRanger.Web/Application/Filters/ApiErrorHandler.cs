using AgeRanger.Web.Application.Exceptions;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace AgeRanger.Web.Application.Filters
{
    public class ApiErrorHandler : ExceptionFilterAttribute
    {
        private static ILog logger = LogManager.GetLogger(typeof(ApiErrorHandler));

        public override void OnException(HttpActionExecutedContext context)
        {
            //log error.
            logger.Error(context.Exception.ToString());

            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            //If UserFriendlyException then pass message back to the client.
            if (context.Exception is UserFriendlyException)
                context.Response.ReasonPhrase = context.Exception.Message;
        }
    }
}