using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RESTService.Lib;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using RabbitMQ.Client;
using System.Threading;
using RabbitMQ.Client.Events;

namespace RESTService.Host
{
    class Program
    {
        public int numeromensajesrecibidos = 0;
        public int numeromensajescontestados = 0;
        public bool servicioniciado = false;

        static void Main(string[] args)
        {
            RestPongServices PongServices = new RestPongServices();
            WebHttpBinding binding = new WebHttpBinding();
            WebHttpBehavior behavior = new WebHttpBehavior();

            WebServiceHost _serviceHost = new WebServiceHost(PongServices, new Uri("http://localhost:8000/PongService"));
            _serviceHost.AddServiceEndpoint(typeof(IRESTPongServices), binding, "");
            _serviceHost.Open();
            Console.ReadKey();
            _serviceHost.Close();
        }

    }
}
