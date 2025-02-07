using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using PiketWebApi;
using PiketWebApi.Api;
using PiketWebApi.Data;
using PiketWebApi.Exceptions;
using PiketWebApi.Services;

var builder = WebApplication.CreateBuilder(args);


if (builder.Environment.IsProduction())
{
    builder.WebHost.UseKestrel(serverOptions =>
    {
        serverOptions.ListenLocalhost(5030);
    });
}

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

string policyName = "all";
builder.Services.AddCors(options =>
{
    options.AddPolicy(policyName, policy =>
    {
        policy.WithOrigins("*")
        .AllowAnyHeader()
        .AllowAnyMethod(); ;
    })
    ;
});



var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddDateOnlyTimeOnlyStringConverters();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
});


//builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(x =>
{
    x.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path} ";
        if (context.Exception != null && context.Exception.GetType() == typeof(BadRequestException))
        {
            BadRequestException badRequest = (BadRequestException)context.Exception;
            context.ProblemDetails.Extensions.Add("errors", badRequest.Errors);
        }
    };
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication();

builder.Services.AddOcphAuthServe(builder.Configuration);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

});
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IClassRoomService, ClassRoomService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IPicketService, PicketService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<ISchoolYearService, SchoolYearService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IStudentAttendaceService, StudentAttendaceService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddProblemDetails();


builder.Services.AddAuthorizationBuilder()
    .AddPolicy("admin_policy", policy =>
        policy
            .RequireRole("admin"));



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{

}

await DataSeeder.SeedData(app);

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseDeveloperExceptionPage();

//if (app.Environment.IsDevelopment())
//{
//}


app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(policyName);
app.UseStaticFiles();    //Serve files from wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "photos")),
    RequestPath = "/photos"
});
app.MapGroup("/api/auth").MapAuthApi().WithOpenApi().WithTags("auth");
app.MapGroup("/api/teacher").MapTeacherApi().WithOpenApi();
app.MapGroup("/api/student").MapStudentApi().WithOpenApi();
app.MapGroup("/api/schoolyear").MapSchoolYearApi().WithOpenApi();
app.MapGroup("/api/department").MapDepartmentApi().WithOpenApi();
app.MapGroup("/api/classroom").MapClassRoomApi().WithOpenApi();
app.MapGroup("/api/schedule").MapScheduleApi().WithOpenApi();
app.MapGroup("/api/picket").MapPickerApi().WithOpenApi();
app.MapGroup("/api/report").MapReportApi().WithOpenApi();
app.MapGroup("/api/studentattendance").MapStudentAttendanceApi().WithOpenApi();


app.Run();
