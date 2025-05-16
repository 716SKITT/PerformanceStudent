using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
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

builder.Services.AddControllers();
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
    options.ListenAnyIP(4200);
    
});
var app = builder.Build();

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

app.MapControllers();

app.Run();
