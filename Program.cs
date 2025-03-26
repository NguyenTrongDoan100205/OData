using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataEx.Data;
using ODataEx.Models;

var builder = WebApplication.CreateBuilder(args);

// Thêm DbContext với SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Thêm OData
builder.Services.AddControllers()
    .AddOData(opt =>
        opt.AddRouteComponents("odata", GetEdmModel()).Filter().Select().Count());

// Thêm Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Thêm CORS (cho phép gọi API từ frontend khác)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Sử dụng CORS
app.UseCors("AllowAllOrigins");

// Cấu hình Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cấu hình pipeline
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

// Tạo dữ liệu ban đầu
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Chỉ dùng EnsureCreated() nếu KHÔNG dùng Migration
    if (dbContext.Database.GetPendingMigrations().Any() == false)
    {
        dbContext.Database.EnsureCreated();
    }

    // Thêm dữ liệu ban đầu nếu chưa có
    if (!dbContext.Books.Any())
    {
        dbContext.Books.AddRange(
            new Book { Title = "Lập trình C#", Author = "Nguyễn Văn Nam", Price = 29.99m },
            new Book { Title = "Học OData", Author = "Trần Thị Kim Dung", Price = 19.99m },
            new Book { Title = "ASP.NET Core", Author = "Lê Văn Cường", Price = 39.99m }
        );
        dbContext.SaveChanges();
    }
}

app.Run();

// Định nghĩa EDM cho OData
static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Book>("Books");
    return builder.GetEdmModel();
}