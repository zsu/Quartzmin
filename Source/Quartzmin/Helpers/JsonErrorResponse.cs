﻿
namespace Quartzmin.Helpers
{
#if (NETSTANDARD || NETCOREAPP || NET6)
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class JsonErrorResponseAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                context.Result = new NewtonsoftJsonResult(new { ExceptionMessage = context.Exception.Message }) { StatusCode = 400 };
                context.ExceptionHandled = true;
            }
        }
    }
#endif

#if NETFRAMEWORK
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web.Http.Filters;
    using Newtonsoft.Json;

    public class JsonErrorResponseAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            if (actionContext.Exception != null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { ExceptionMessage = actionContext.Exception.GetBaseException().Message }), Encoding.UTF8, "application/json")
                };
            }
        }
    }
#endif

}
