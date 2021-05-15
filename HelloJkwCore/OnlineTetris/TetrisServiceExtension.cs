using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineTetris
{
    public static class TetrisServiceExtension
    {
        public static void AddTetrisService(this IServiceCollection services, IConfiguration configuration)
        {
            //var tetrisOption = new TetrisOption();
            //configuration.GetSection("OnlineTetris").Bind(tetrisOption);

            //services.AddSingleton(tetrisOption);
            services.AddSingleton<ITetrisService, TetrisService>();
        }
    }
}
