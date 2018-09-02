using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace D365WebHookSamples.Plugins
{
    [KnownType(typeof(Data))]
    public class CustomObject : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private Guid serviceEndpointId;

        public CustomObject(string unsecureConfig, string secureConfig)
        {
            if (string.IsNullOrEmpty(secureConfig) || !Guid.TryParse(secureConfig, out serviceEndpointId))
            {
                if (string.IsNullOrEmpty(unsecureConfig) || !Guid.TryParse(unsecureConfig, out serviceEndpointId))
                {
                    throw new InvalidPluginExecutionException("Service endpoint IS should be passed as config!");
                }
            }
        }
        #endregion
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            IServiceEndpointNotificationService cloudService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));

            try
            {
                var myContext = context;

                Data myData = new Data();
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    myData.Contact = context.InputParameters["Target"] as Entity;
                    myData.Success = true;
                }
                else
                {
                    myData.Success = false;
                }
                myContext.OutputParameters.Add(new KeyValuePair<string, object>("MyData", "SomeString"));

                tracingService.Trace("Posting the execution context.");

                string response = cloudService.Execute(new EntityReference("serviceendpoint", serviceEndpointId), myContext);

                if (!string.IsNullOrEmpty(response))
                {
                    tracingService.Trace("Response = {0}", response);
                }
                tracingService.Trace("Done.");
            }
            catch (Exception e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}