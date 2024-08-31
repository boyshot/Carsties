using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(op =>
{
  op.Authority = builder.Configuration["IdentityServiceUrl"];
  op.RequireHttpsMetadata = false;
  op.TokenValidationParameters.ValidateAudience = false;
  op.TokenValidationParameters.NameClaimType = "username";
});

builder.Services.AddCors(op =>
{
  op.AddPolicy("customPolicy", b =>
  {
    b.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["ClientApp"]);
  });
});

var app = builder.Build();

app.UseCors();

app.MapReverseProxy();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
