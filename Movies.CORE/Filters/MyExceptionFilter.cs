using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Movies.CORE.Filters
{
    public class MyExceptionFilter: ExceptionFilterAttribute
    {
        private readonly ILogger<MyExceptionFilter> logger;

        public MyExceptionFilter(ILogger<MyExceptionFilter> logger)
        {
            this.logger = logger;
        }
        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
