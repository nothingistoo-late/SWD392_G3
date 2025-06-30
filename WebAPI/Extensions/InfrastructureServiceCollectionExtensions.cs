using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories.Implementations;
using Repositories.Implements;
using Repositories.WorkSeeds.Interfaces;
using Services.Helpers.Mappers;
using System.Text;

namespace WebAPI.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Cấu hình Settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // 2. DbContext và CORS
            services.AddDbContext<SWD392_G3DBcontext>(opt =>
                opt.UseSqlServer(
                    configuration.GetConnectionString("SWD392_G3"),
                    sql => sql.MigrationsAssembly("Repositories")));
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", b => b
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            // 3. Identity & Authentication
            services.AddIdentity<User, Role>(opts =>
            {
                // Bắt buộc phải xác thực email mới cho SignIn
                opts.SignIn.RequireConfirmedEmail = true;
                opts.ClaimsIdentity.UserNameClaimType = ClaimTypes.Name;
                opts.ClaimsIdentity.RoleClaimType = ClaimTypes.Role;
                opts.Lockout.MaxFailedAccessAttempts = 5;
                opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opts.Lockout.AllowedForNewUsers = true;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireDigit = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequiredLength = 4;
            })
            .AddEntityFrameworkStores<SWD392_G3DBcontext>()
            .AddDefaultTokenProviders();

            var jwt = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                      ?? throw new InvalidOperationException("JWT key is not configured.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.ValidIssuer,
                    ValidAudience = jwt.ValidAudience,
                    IssuerSigningKey = key
                };
                opts.Events = new JwtBearerEvents
                {
                    OnChallenge = ctx =>
                    {
                        ctx.HandleResponse();
                        ctx.Response.StatusCode = 401;
                        ctx.Response.ContentType = "application/json";
                        var res = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            message = "You are not authorized. Please authenticate."
                        });
                        return ctx.Response.WriteAsync(res);
                    }
                };
            });
            //.AddGoogle(opts =>
            //{
            //    opts.ClientId = configuration["Authentication:Google:ClientId"];
            //    opts.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            //});

            // 4. Repositories & Domain Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<ICurrentTime, CurrentTime>();
            services.AddScoped<IRepositoryFactory, RepositoryFactory>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IStaffScheduleRepository, StaffScheduleRepository>();
            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();


            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserEmailService, UserEmailService>();
            services.AddScoped<IOrderService, OrderService>();  
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<ICustomerService, CustomerService>();

            // 5. Email + Quartz
            services.AddEmailServices(opts =>
                configuration.GetSection("EmailSettings").Bind(opts)
            );

            // 6. Controllers
            services.AddControllers();

            // 7. Mapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }
    }
}
