using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatResults.AspNetCore;
using FlatResults.WebSample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlatResults.WebSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddFlatResultsFormatter();
            ConfigFlatResults();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private static void ConfigFlatResults()
        {
            DocumentMapperConfig.NewConfig<CategoryModel>().WithTypeName("category").MapWithDetaults();
            DocumentMapperConfig.NewConfig<ProductModel>().WithTypeName("product").MapWithDetaults();
            DocumentMapperConfig.NewConfig<UnitOfMeasureModel>().WithTypeName("unitOfMeasure").MapWithDetaults();

            DocumentMapperConfig.AddWrapperType(typeof(ResultModel<>), r => r.Data, r =>
            {
                var result = new Dictionary<string, object>();
                if (r.TotalCount > 0)
                    result.Add("TotalCount", r.TotalCount);
                return result;
            });
        }
    }
}
