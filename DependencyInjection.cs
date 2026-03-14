using VocabTrainer.Api.Storage;
using VocabTrainer.Api.Settings;
using VocabTrainer.Api.Abstractions;
using VocabTrainer.Api.Abstractions.Storage;

namespace VocabTrainer.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
        {
            // ------------------------------ Db ------------------------------
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // ------------------------------ Settings ------------------------------
            services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName));
            services.Configure<PaginationSettings>(config.GetSection(PaginationSettings.SectionName));
            services.Configure<StorageSettings>(config.GetSection(StorageSettings.SectionName));
            services.Configure<ElevenLabsSettings>(config.GetSection(ElevenLabsSettings.SectionName));

            // ------------------------------ Auth ------------------------------
            var jwt = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                      ?? throw new InvalidOperationException("JwtSettings missing");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
                    };
                });

            services.AddAuthorization();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
            services.AddSingleton<IFileStorage, LocalFileStorage>();

            // ------------------------------ External Providers ------------------------------ ✅
            // GoogleTranslateFreeApi Provider
            services.AddScoped<ITranslationProvider, GTranslateProvider>();

            // ------------------------------ Application Services ------------------------------
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ILanguagesService, LanguagesService>();
            services.AddScoped<IWordsService, WordsService>();
            services.AddScoped<IQuizService, QuizService>();
            services.AddScoped<IStatsService, StatsService>();

            services.AddScoped<IAdminCodesService, AdminCodesService>();
            services.AddScoped<IAdminLanguagesService, AdminLanguagesService>();
            services.AddScoped<IAdminUsersService, AdminUsersService>();
            services.AddScoped<ITranslationProvider, GTranslateProvider>();
            services.AddSingleton<ITtsProvider, WindowsTtsProvider>();
            services.AddSingleton<ILanguageCatalog, LanguageCatalog>();


            // ------------------------------ Background jobs ------------------------------
            services.AddHostedService<ExpireCodesBackgroundService>();
            services.AddHostedService<GuestCleanupBackgroundService>();

            // ------------------------------ Misc ------------------------------
            services.AddSignalR();
            services.AddCors(opt =>
                opt.AddPolicy("AllowAll", p =>
                    p.AllowAnyOrigin()
                     .AllowAnyMethod()
                     .AllowAnyHeader()));

            return services;
        }
    }
}