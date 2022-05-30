using Microsoft.AspNet.WebHooks;
using Owin;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace NekoChattingBot.Webhook
{
    public class WebApiConfig
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            // Set the assembly resolver so that WebHooks receiver controller is loaded.
            WebHookAssemblyResolver assemblyResolver = new WebHookAssemblyResolver();
            config.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            appBuilder.UseWebApi(config);

            // Initialize Custom WebHook receiver
            config.InitializeReceiveCustomWebHooks();
        }
    }
    public class LiveWebHookHandler : WebHookHandler
    {
        public LiveWebHookHandler()
        {
            this.Receiver = CustomWebHookReceiver.ReceiverName;
        }

        public override Task ExecuteAsync(string generator, WebHookHandlerContext context)
        {
            // Get data from WebHook
            CustomNotifications data = context.GetDataOrDefault<CustomNotifications>();

            // Get data from each notification in this WebHook
            foreach (IDictionary<string, object> notification in data.Notifications)
            {
                // Process data
                System.Console.WriteLine(notification);
            }

            return Task.FromResult(true);
        }
    }
}
