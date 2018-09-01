using Microsoft.Xrm.Sdk;
using System;

namespace D365WebHookSamples.Plugins
{
    public class SimmplePassThrough : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private Guid serviceEndpointId;

        public SimmplePassThrough(string unsecureConfig, string secureConfig)
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
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            IServiceEndpointNotificationService cloudService = (IServiceEndpointNotificationService) serviceProvider.GetService(typeof(IServiceEndpointNotificationService));
            
            try
            {
                tracingService.Trace("Posting the execution context.");
                string response = cloudService.Execute(new EntityReference("serviceendpoint", serviceEndpointId), context);
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