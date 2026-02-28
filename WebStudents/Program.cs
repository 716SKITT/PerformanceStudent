using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Prometheus;
using System.Text.Json.Serialization;
using WebStudents.src.EF;
using WebStudents.src.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ProffessorService>();
builder.Services.AddScoped<GradeService>();
builder.Services.AddScoped<AttendanceService>();
builder.Services.AddScoped<AssignmentService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<AcademicYearService>();
builder.Services.AddScoped<SemesterService>();
builder.Services.AddScoped<StudentGroupService>();
builder.Services.AddScoped<DisciplineService>();
builder.Services.AddScoped<DisciplineOfferingService>();
builder.Services.AddScoped<GradeSheetService>();
builder.Services.AddScoped<FinalGradeService>();
builder.Services.AddScoped<AccessPolicyService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5006);
});
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
    db.Database.Migrate();
    DbSeeder.Seed(db);
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
    c.RoutePrefix = "swagger";
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowFrontend");

app.UseRouting();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics("/metrics");

app.Run();
