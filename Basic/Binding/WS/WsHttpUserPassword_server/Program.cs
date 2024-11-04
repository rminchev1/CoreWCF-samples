using System.Diagnostics;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// The main namespace for the WSHttpUserPassword server application.
/// </summary>
namespace NetCoreServer
{
    /// <summary>
    /// The main entry point for the WSHttpUserPassword server application.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The HTTP port number for the server.
        /// </summary>
        public const int HTTP_PORT = 8088;
        /// <summary>
        /// The HTTPS port number for the server.
        /// </summary>
        public const int HTTPS_PORT = 8443;

        /// <summary>
        /// The main method that starts the web host.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        static void Main(string[] args)
        {
            IWebHost host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        /// <summary>
        /// Creates a web host builder configured to use Kestrel and the WSHttpUserPassword startup class.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>An IWebHostBuilder instance.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options => // Configure Kestrel server options
            {
                options.ListenLocalhost(HTTP_PORT); // Listen on HTTP port
                options.ListenLocalhost(HTTPS_PORT, listenOptions => // Listen on HTTPS port
                {
                    listenOptions.UseHttps(); // Use HTTPS for secure communication
                    if (Debugger.IsAttached)
                    {
                        listenOptions.UseConnectionLogging(); // Enable connection logging if debugging
                    }
                });
            })

            // Use the WSHttpUserPassword startup class for configuring the application
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
                logging.SetMinimumLevel(LogLevel.Debug);
            })
            .UseStartup<WSHttpUserPassword>();
    }
}
