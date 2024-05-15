

using EventBus.Base.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.IntegrationEvents.Events;
using NotificationService.IntegrationEvents.Handlers;
using NotificationService.ServiceRegistration;

ServiceCollection services = new ServiceCollection();
services.AddAppServices();
ServiceProvider sp =  services.BuildServiceProvider();


IEventBus eventBus = sp.GetService<IEventBus>();
eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();
eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();

Console.WriteLine("Application is running...!");
Console.ReadLine();

