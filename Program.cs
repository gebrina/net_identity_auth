using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Net_Identity_Auth.DbContext;
using Net_Identity_Auth.Models;

var builder = WebApplication.CreateBuilder(args);

var DbCon = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw
new ArgumentException("Invalid connection string");
// add services
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(DbCon);
});

builder.Services.AddIdentity<ApplicatoinUser, IdentityRole>()
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "User auth api",
        Description = "This api used to authenticate and authorize users",
        Version = "1",
        Contact = new OpenApiContact
        {
            Email = "youremail@emailservice.com"
        }
    });
});

var app = builder.Build();

// add middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.MapControllers();

app.Run();