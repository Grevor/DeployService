using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Viking.DeploymentService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var service = new Service1();
            if (Environment.UserInteractive)
            {
                service.StartInteractive();
                Console.WriteLine("Press any key to stop the service...");
                Console.ReadKey();
                service.StopInteractive();
            }
            else
            {
                ServiceBase.Run(new[] { service });
            }
        }
    }
}
