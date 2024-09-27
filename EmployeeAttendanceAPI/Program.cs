using Microsoft.EntityFrameworkCore;
using EmployeeAPI.Data;
using EmployeeAPI.DtoMapping;
using EmployeeAPI.Repository;
using EmployeeAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;


var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
string cs = builder.Configuration.GetConnectionString("ConStr");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(cs));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers().AddRazorRuntimeCompilation();
builder.Services.AddScoped<IEmailSender, EmailService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSetting"));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});
builder.Services.AddAuthorization(); // Add Authorization middleware

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "EmployeeAPI", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

//builder.Services.AddSession(Options =>
//{
//    Options.IdleTimeout = TimeSpan.FromMinutes(30);
//    Options.Cookie.HttpOnly = true;
//    Options.Cookie.IsEssential = true;

//});
var app = builder.Build();

using var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
await SeedRolesAsync(roleManager);

async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
if (!await roleManager.RoleExistsAsync("Admin"))
{
await roleManager.CreateAsync(new IdentityRole("Admin"));
}

if (!await roleManager.RoleExistsAsync("HR"))
{
await roleManager.CreateAsync(new IdentityRole("HR"));
}

if (!await roleManager.RoleExistsAsync("Employee"))
{
await roleManager.CreateAsync(new IdentityRole("Employee"));
}

if (!await roleManager.RoleExistsAsync("Manager"))
{
await roleManager.CreateAsync(new IdentityRole("Manager"));
}
}


//configure the http request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting(); // Add routing middleware
app.UseAuthentication();
app.UseAuthorization(); // Add authorization middleware

app.MapControllers();

app.Run();
