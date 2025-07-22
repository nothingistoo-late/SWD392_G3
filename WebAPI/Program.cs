var builder = WebApplication.CreateBuilder(args);

// 1) Đăng ký toàn bộ hạ tầng + controllers
builder.Services
      .AddInfrastructure(builder.Configuration)
      .AddSwaggerServices();


builder.Services.Configure<VnPaySettings>(builder.Configuration.GetSection("VnPaySettings"));
builder.Services.AddSingleton<VnPayService>();

var app = builder.Build();

// 2) Áp dụng pipeline (migrations, routing, auth, map controllers…) và swagger
var applicationBuilder = await app.UseApplicationPipeline();
applicationBuilder.UseSwaggerPipeline();

app.Run();
