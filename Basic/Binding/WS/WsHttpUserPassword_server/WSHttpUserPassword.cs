using System;
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Contract;

/// <summary>
/// This namespace contains the server-side implementation for the WSHttpUserPassword service.
/// </summary>
namespace NetCoreServer
{
    /// <summary>
    /// Configures the WSHttpUserPassword service with CoreWCF.
    /// </summary>
    public class WSHttpUserPassword
    {
        /// <summary>
        /// Configures the services required for the WSHttpUserPassword service.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            //Enable CoreWCF Services, with metadata (WSDL) support
            services.AddServiceModelServices()
                .AddServiceModelMetadata();
        }

        /// <summary>
        /// Configures the application pipeline for the WSHttpUserPassword service.
        /// </summary>
        /// <param name="app">The application builder to configure.</param>
        public void Configure(IApplicationBuilder app)
        {
            // Create a WSHttpBinding with TransportWithMessageCredential security mode
            var wsHttpBindingWithCredential = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
            wsHttpBindingWithCredential.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            app.UseServiceModel(builder =>
            {
                // Add the Echo Service with specified endpoints
                builder.AddService<EchoService>(serviceOptions =>
                {
                    // Set a base address for all bindings to the service, and WSDL discovery
                    serviceOptions.BaseAddresses.Add(new Uri($"http://localhost:{Program.HTTP_PORT}/EchoService"));
                    serviceOptions.BaseAddresses.Add(new Uri($"https://localhost:{Program.HTTPS_PORT}/EchoService"));
                })
                // Add WSHttpBinding endpoints
                .AddServiceEndpoint<EchoService, IEchoService>(new WSHttpBinding(SecurityMode.None), "/wsHttp")
                .AddServiceEndpoint<EchoService, IEchoService>(new WSHttpBinding(SecurityMode.Transport), "/wsHttp")
                .AddServiceEndpoint<EchoService, IEchoService>(wsHttpBindingWithCredential, "/wsHttpUserPassword", ep => { ep.Name = "AuthenticatedEP"; });

                // Configure custom username/password validator for the EchoService
                builder.ConfigureServiceHostBase<EchoService>(CustomUserNamePasswordValidator.AddToHost);

                // Enable WSDL metadata to be available over HTTP and HTTPS
                var serviceMetadataBehavior = app.ApplicationServices.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
                serviceMetadataBehavior.HttpGetEnabled = serviceMetadataBehavior.HttpsGetEnabled = true;
            });
        }
    }
}
