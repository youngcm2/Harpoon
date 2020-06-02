using System;
using Harpoon.Registrations.EFStorage;
using Harpoon.Sender;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace Harpoon.Registrations.Mongo
{
    /// <summary>
    /// A set of extensions methods on <see cref="IHarpoonBuilder"/> to allow the usage of EF Core
    /// </summary>
    public static class ServicesCollectionsExtensions
    {
        /// <summary>
        /// Registers <see cref="WebHookStore{TContext}"/> as <see cref="IWebHookStore"/> and <see cref="WebHookRegistrationStore{TContext}"/> as <see cref="IWebHookRegistrationStore"/>.
        /// TWebHookTriggerProvider is registered as singleton
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <typeparam name="TWebHookTriggerProvider"></typeparam>
        /// <param name="harpoon"></param>
        /// <returns></returns>
        public static IHarpoonBuilder RegisterWebHooksUsingMongoStorage<TWebHookTriggerProvider>(this IHarpoonBuilder harpoon, string connectionString, string databaseName)
            where TWebHookTriggerProvider : class, IWebHookTriggerProvider
        {
            harpoon.Services.TryAddSingleton<IWebHookTriggerProvider, TWebHookTriggerProvider>();
            return harpoon.RegisterWebHooksUsingMongoStorage(connectionString, databaseName);
        }

        /// <summary>
        /// Registers <see cref="WebHookStore{TContext}"/> as <see cref="IWebHookStore"/> and <see cref="WebHookRegistrationStore{TContext}"/> as <see cref="IWebHookRegistrationStore"/>.
        /// TWebHookTriggerProvider needs to be configured.
        /// </summary>
        /// <param name="harpoon"></param>
        /// <param name="connectionString"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static IHarpoonBuilder RegisterWebHooksUsingMongoStorage(this IHarpoonBuilder harpoon, string connectionString, string databaseName)
        {
            harpoon.Services.TryAddScoped<IPrincipalIdGetter, DefaultPrincipalIdGetter>();
            harpoon.Services.TryAddScoped<IWebHookStore, WebHookStore>();
            harpoon.Services.TryAddScoped<IWebHookRegistrationStore, WebHookRegistrationStore>();
            harpoon.Services.TryAddSingleton<IMongoClient>(provider => new MongoClient(connectionString));
            harpoon.Services.TryAddSingleton<IMongoDatabase>(provider => provider.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
            
            harpoon.Services.TryAddScoped<IMongoContext<WebHook>, WebHookRepository>();
            harpoon.Services.TryAddScoped<IMongoContext<WebHookFilter>, WebHookFilterRepository>();
            harpoon.Services.TryAddScoped<IMongoContext<WebHookLog>, WebHookLogRepository>();
            harpoon.Services.TryAddScoped<IMongoContext<WebHookNotification>, WebHookNotificationRepository>();
            

            return harpoon;
        }

        /// <summary>
        /// Registers <see cref="DefaultSecretProtector"/> as <see cref="ISecretProtector"/>
        /// </summary>
        /// <param name="harpoon"></param>
        /// <param name="dataProtection"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        public static IHarpoonBuilder UseDefaultDataProtection(this IHarpoonBuilder harpoon, Action<IDataProtectionBuilder> dataProtection, Action<DataProtectionOptions> setupAction)
        {
            if (dataProtection == null)
            {
                throw new ArgumentNullException("Data protection configuration is required.", nameof(dataProtection));
            }

            harpoon.Services.TryAddScoped<ISecretProtector, DefaultSecretProtector>();
            dataProtection(harpoon.Services.AddDataProtection(setupAction));
            return harpoon;
        }

        /// <summary>
        /// Registers services to use <see cref="MongoWebHookSender"/> as the default <see cref="IQueuedProcessor{IWebHookWorkItem}"/>.
        /// </summary>
        /// <param name="harpoon"></param>
        /// <param name="senderPolicy">This parameter lets you define your retry policy</param>
        /// <returns></returns>
        public static IHarpoonBuilder UseDefaultMongoWebHookWorkItemProcessor<TContext>(this IHarpoonBuilder harpoon, Action<IHttpClientBuilder> senderPolicy)
        {
            if (senderPolicy == null)
            {
                throw new ArgumentNullException(nameof(senderPolicy));
            }

            harpoon.Services.TryAddSingleton<ISignatureService, DefaultSignatureService>();
            harpoon.Services.TryAddScoped<IQueuedProcessor<IWebHookWorkItem>, MongoWebHookSender>();
            var builder = harpoon.Services.AddHttpClient<IQueuedProcessor<IWebHookWorkItem>, MongoWebHookSender>();
            senderPolicy(builder);
            return harpoon;
        }

        /// <summary>
        /// Registers <see cref="MongoNotificationProcessor"/> as the default <see cref="IQueuedProcessor{IWebHookNotification}"/>.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="harpoon"></param>
        /// <returns></returns>
        public static IHarpoonBuilder UseDefaultMongoNotificationProcessor<TContext>(this IHarpoonBuilder harpoon)
        {
            harpoon.Services.TryAddScoped<IQueuedProcessor<IWebHookNotification>, MongoNotificationProcessor>();
            return harpoon;
        }

        /// <summary>
        /// Registers <see cref="MongoNotificationProcessor"/> as the default <see cref="IWebHookService"/>, allowing for a synchronous treatment of <see cref="IWebHookNotification"/>
        /// </summary>
        /// <param name="harpoon"></param>
        /// <returns></returns>
        public static IHarpoonBuilder ProcessNotificationsSynchronouslyUsingMongoDefault<TContext>(this IHarpoonBuilder harpoon)
        {
            harpoon.Services.TryAddScoped<IWebHookService, MongoNotificationProcessor>();
            return harpoon;
        }
    }
}