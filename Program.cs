using k8sFormApp.Services;
using StackExchange.Redis;

namespace k8sFormApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.Configuration.AddEnvironmentVariables();

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(8081); // Match the EXPOSE and Docker -p port
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Redis configuration
            var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "redis:6379";
            builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                //return ConnectionMultiplexer.Connect(redisConnectionString);

                var configuration = ConfigurationOptions.Parse(redisConnectionString);
                configuration.AbortOnConnectFail = false; // Important for Kubernetes
                configuration.ConnectRetry = 3;
                configuration.ConnectTimeout = 5000;
                return ConnectionMultiplexer.Connect(configuration);
            });

            builder.Services.AddScoped<IRedisService, RedisService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
