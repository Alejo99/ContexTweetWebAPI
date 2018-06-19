using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ContexTweet.Data;
using Microsoft.EntityFrameworkCore;
using ContexTweet.Configuration;

namespace ContexTweet
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
            #region Configuration options

            //Enable options
            services.AddOptions();
            //Add paging options
            services.Configure<PagingOptions>(Configuration.GetSection("Paging"));

            #endregion

            #region Database and repositories

            //Application DB context, uses identity framework
            services.AddDbContext<ContexTweetDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ContexTweetDb")));
            //Tweet repository
            services.AddScoped<ITweetRepository, EFTweetRepository>();
            //Named entities repository
            services.AddScoped<INamedEntityRepository, EFNamedEntityRepository>();

            #endregion

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
