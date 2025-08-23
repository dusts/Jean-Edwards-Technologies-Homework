namespace Blazor_Backend_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Read BlazorWebAssemblyApp from configuration
            var blazorWebAssemblyApp = builder.Configuration["BlazorWebAssemblyApp"]
                ?? throw new InvalidOperationException("BlazorWebAssemblyApp is missing in configuration.");
            
            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddHttpClient();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazor", policy =>
                {
                    policy.WithOrigins(blazorWebAssemblyApp) // Blazor app URL
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseCors("AllowBlazor");
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
