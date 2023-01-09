
using System.Reflection;
using Application.Services.AdditionalServiceService;
using Application.Services.AuthService;
using Application.Services.AzureBus;
using Application.Services.CarService;
using Application.Services.CustomerService;
using Application.Services.FindeksCreditRateService;
using Application.Services.ImageService;
using Application.Services.ImageService.Enums;
using Application.Services.ImageService.StorageService;
using Application.Services.InvoiceService;
using Application.Services.ModelService;
using Application.Services.RentalService;
using Application.Services.RentalsIAdditionalServiceService;
using Application.Services.Repositories;
using Application.Services.UserService;
using Core.Application.Pipelines.Authorization;
using Core.Application.Pipelines.Caching;
using Core.Application.Pipelines.Logging;
using Core.Application.Pipelines.Transaction;
using Core.Application.Pipelines.Validation;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Logging.Serilog;
using Core.CrossCuttingConcerns.Logging.Serilog.Logger;
using Core.ElasticSearch;
using Core.Mailing;
using Core.Mailing.MailKitImplementations;
using Core.Persistence.Repositories;
using Core.Security.Entities;
using Core.Storage;
using Core.Storage.Azure;
using Core.Storage.Local;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheRemovingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionScopeBehavior<,>));


        services.AddScoped<IAdditionalServiceService, AdditionalServiceManager>();
        services.AddScoped<IAuthService, AuthManager>();
        services.AddScoped<ICarService, CarManager>();
        services.AddScoped<ICustomerService, CustomerManager>();
        services.AddScoped<IFindeksCreditRateService, FindeksCreditRateManager>();
        services.AddScoped<IInvoiceService, InvoiceManager>();
        services.AddScoped<IModelService, ModelManager>();
        services.AddScoped<IRentalService, RentalManager>();
        services.AddScoped<IRentalsAdditionalServiceService, RentalsAdditionalServiceManager>();
        services.AddScoped<IUserService, UserManager>();
        //services.AddSingleton<Entity, User>();
        //services.AddSingleton<Entity, Invoice>();

        services.AddSingleton<IAzureBusService<User>, AzureBusManager<User>>();
        services.AddSingleton<IAzureBusService<Rental>, AzureBusManager<Rental>>();
        services.AddSingleton<IAzureBusService<Invoice>, AzureBusManager<Invoice>>();
        services.AddSingleton<IMailService, MailKitMailService>();
        services.AddSingleton<LoggerServiceBase, FileLogger>();
       
        //services.AddSingleton<IElasticSearch, ElasticSearchManager>();




        services.AddScoped<IStorageService, StorageServices>();
        services.AddScoped<IAzureStorage, AzureStorage>();
        services.AddScoped<ILocalStorage, LocalStorage>();
        return services;
    }
    public static IServiceCollection AddStorage<T>(this IServiceCollection services) where T : Storage, IStorage
    {
        services.AddScoped<IStorage, T>();
        return services;
    }
    //public static IServiceCollection AddStorage(this IServiceCollection services, StorageType storageType)
    //{
    //    switch (storageType)
    //    {
    //        case StorageType.Locale:
    //            services.AddScoped<IStorage, LocalStorage>();
    //            break;
    //        case StorageType.Azure:
    //            services.AddScoped<IStorage, AzureStorage>();
    //            break;
    //        //case StorageType.AWS:
    //        //    break;
    //        default:
    //            services.AddScoped<IStorage, LocalStorage>();
    //            break;
    //    }

    //    return services;
    //}
    public static IServiceCollection AddSubClassesOfType(
        this IServiceCollection services,
        Assembly assembly,
        Type type,
        Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
    {
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        foreach (var item in types)
        {
            if (addWithLifeCycle == null)
            {
                services.AddScoped(item);
            }
            else
            {
                addWithLifeCycle(services, type);
            }
        }
        return services;
    }
}