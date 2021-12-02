using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.CORE.Filters
{
    public class MyActionFilter : IActionFilter
    {
        private readonly ILogger<MyActionFilter> logger;

        public MyActionFilter(ILogger<MyActionFilter> logger)
        {
            this.logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            logger.LogWarning("OnActionExecuted");
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            logger.LogWarning("OnActionExecuted");
        }
    }
}
