using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using grapetest2.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using grapetest2.Controllers;
using grapetest2.Data.Repositories;

namespace grapetest2
{
  public class Startup
  {
    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {

      services.Configure<IdentityOptions>(options =>
      {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 0;
        options.User.RequireUniqueEmail = false;
      });

      services.Configure<CookiePolicyOptions>(options =>
      {
        // This lambda determines whether user consent for non-essential cookies is needed for a given request.
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.AddScoped(typeof(CustomerConversationRepository));

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

      string dbname = Guid.NewGuid().ToString();
      services.AddDbContext<ApplicationDbContext>(options =>
      {
        options.UseInMemoryDatabase(dbname);
      });
      services.AddDefaultIdentity<IdentityUser>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

      services.AddSignalR();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseCookiePolicy();
      app.UseCors();
      app.UseAuthentication();

      app.UseSignalR(routes =>
      {
        routes.MapHub<ChatHub>("/chatHub");
      });

      app.UseMvc(routes => { routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}"); });

      this.Seed(app);
    }

    private void Seed(IApplicationBuilder appBuilder)
    {

      using (var serviceScope = appBuilder.ApplicationServices.CreateScope())
      {
        ApplicationDbContext context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        IdentityUser adminuser = new IdentityUser();
        adminuser.UserName = "operator@test.hu";
        adminuser.Email = "operator@test.hu";
        adminuser.EmailConfirmed = true;
        adminuser.TwoFactorEnabled = false;
        adminuser.LockoutEnabled = false;
        adminuser.NormalizedUserName = "operator";

        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        Task<IdentityResult> user = userManager.CreateAsync(adminuser, "operator");
      }
    }

  }
}
