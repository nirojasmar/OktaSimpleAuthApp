using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OktaSimpleAuth
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
            services.AddControllersWithViews();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = "/login";
                options.AccessDeniedPath = "/denied";
                options.Events = new CookieAuthenticationEvents()
                {
                    OnSigningIn = async context =>
                    {
                        var scheme = context.Properties.Items.Where(k => k.Key == ".AuthScheme").FirstOrDefault();
                        var claim = new Claim(scheme.Key, scheme.Value);
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        claimsIdentity.AddClaim(claim);
                    }
                };
            })
                .AddOpenIdConnect("google", options =>
                {
                    options.Authority = "https://accounts.google.com";
                    options.ClientId = "552672253869-inb03pj4244k0c6hjmqofo5437hm49po.apps.googleusercontent.com";
                    options.ClientSecret = "wEXH5McdwNsfFacEBoXNiHU_";
                    options.CallbackPath = "/auth";
                    options.SaveTokens = true;
                    options.Prompt = "consent";
                    options.Events = new OpenIdConnectEvents()
                    {
                        OnTokenValidated = async context =>
                        {
                            if (context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value == "104633631935742496450")
                            {
                                var claim = new Claim(ClaimTypes.Role, "Admin");
                                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                                claimsIdentity.AddClaim(claim);
                            }
                            var claims = context.Principal.Claims;
                        }
                    };
                }).AddOpenIdConnect("okta", options => {
                    options.Authority = "https://dev-82557555.okta.com/oauth2/default";
                    options.ClientId = "0oa1e2e9cdZPuBeXs5d7";
                    options.ClientSecret = "_YwZ03PYEqbKK-y8nfH5f-PLm9KOVA5J9w-Tv39g";
                    options.CallbackPath = "/okta-auth";
                    options.ResponseType = "code";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
