using System.Reflection;
using FreeSql;
using Microsoft.OpenApi.Models;
using YouthApartmentServer.Repositories.IUser;
using YouthApartmentServer.Repositories.IRole;
using Mapster;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// 添加控制器服务
builder.Services.AddControllers();

// 配置CORS策略
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// 配置Swagger服务
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "青年公寓管理系统 API",
        Version = "v1",
        Description = "青年公寓管理系统的后端API接口文档",
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.EnableAnnotations();
});

//接入Freesql
Func<IServiceProvider, IFreeSql> fsqlFactory = r =>
{
    string connectionString = "Server=127.0.0.1;Port=3306;Database=youth_apartment_db;User ID=root;Password=123456;Charset=utf8mb4;SslMode=none;AllowPublicKeyRetrieval=true;Min pool size=1";
    IFreeSql fsql = new FreeSql.FreeSqlBuilder()
        .UseConnectionString(FreeSql.DataType.MySql, connectionString)
        .UseAdoConnectionPool(true)
        .UseMonitorCommand(cmd => Console.WriteLine($"Sql：{cmd.CommandText}"))
        .UseAutoSyncStructure(true)
        .Build();
    return fsql;
};
builder.Services.AddSingleton<IFreeSql>(fsqlFactory);
builder.Services.AddScoped<UnitOfWorkManager>(sp =>
    new UnitOfWorkManager(sp.GetRequiredService<IFreeSql>()));

// 注册仓储（FreeSql 的基类仓储）
builder.Services.AddFreeRepository(typeof(UserRepository).Assembly);

// 为 *Service / *Repository 自动注册
builder.Services.AddConventionServices(Assembly.GetExecutingAssembly());


#region Mapster
// 使用 Mapster：注册全局映射配置（替代 AutoMapper/ServiceMapper）
YouthApartmentServer.Profiles.UserProfile.Register(TypeAdapterConfig.GlobalSettings);
// 注册 UserRole 的映射配置
YouthApartmentServer.Profiles.UserRoleProfile.Register(TypeAdapterConfig.GlobalSettings);
//注册Property映射
YouthApartmentServer.Profiles.PropertyProfile.Register(TypeAdapterConfig.GlobalSettings);
#endregion

var app = builder.Build();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    await next();
});

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    // 启用Swagger中间件
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "青年公寓管理系统 API v1");
        options.RoutePrefix = "swagger"; // 设置Swagger UI的路由前缀
        options.DocumentTitle = "青年公寓管理系统 API 文档";
    });
}

// 启用CORS中间件
app.UseCors("AllowAllOrigins");

// 映射 Resources 目录为静态资源，可通过 /resources/* 访问
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(app.Environment.ContentRootPath, "Resources")),
    RequestPath = "/resources"
});

// 配置根路径重定向到Swagger文档（使用中间件）
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" && context.Request.Method == "GET")
    {
        context.Response.Redirect("/swagger");
        return;
    }
    await next();
});

app.MapControllers();
app.Run();

// 约定式注册扩展：扫描当前程序集，自动注册 *Service / *Repository 实现类
public static class ServiceRegistrationExtensions
{
    public static void AddConventionServices(this IServiceCollection services, Assembly assembly)
    {
        var candidates = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && (t.Name.EndsWith("Service") || t.Name.EndsWith("Repository")));

        foreach (var impl in candidates)
        {
            var iface = impl.GetInterfaces().FirstOrDefault(i => i.Name == "I" + impl.Name);
            if (iface != null)
            {
                services.AddScoped(iface, impl);
            }
        }
    }
}






