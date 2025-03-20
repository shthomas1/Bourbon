using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ✅ Enable Sessions (Fixed SameSite Issue)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // ✅ Fixes session persistence in browsers
});

// ✅ CORS Policy (Fixed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .SetIsOriginAllowed(origin => true) // ✅ Allows requests from any frontend
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials() // ✅ Ensures session cookies are included in requests
    );
});

// ✅ Swagger for API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Database Connection
builder.Services.AddScoped<IDbConnection>(sp =>
    new MySqlConnection("server=hngomrlb3vfq3jcr.cbetxkdyhwsb.us-east-1.rds.amazonaws.com;" +
                        "database=yep373vb1cf2kgw4;" +
                        "user=igdr2swvcwmsauzw;" +
                        "password=i7vxdcy8tm67omyt;"));

var app = builder.Build();

// ✅ Middleware Order (Fixed)
app.UseCors("AllowFrontend"); // ✅ CORS must be before Routing!
app.UseSession(); // ✅ Ensures session cookies are sent properly
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ✅ Swagger Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseDefaultFiles();
app.UseStaticFiles();

// ✅ Map API Controllers
app.MapControllers();

app.Run();
