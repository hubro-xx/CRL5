using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRL.Core.ConsulClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CRL.Ocelot.Controllers
{


    [Produces("application/json")]
    [Route("consul/[action]")]
    public class ConsulController : ControllerBase
    {
        private readonly Client _client;
        private readonly ILogger<ConsulController> _logger;

        public ConsulController(ILogger<ConsulController> logger, Client client)
        {
            _logger = logger;
            _client = client;
        }
        [HttpPut]
        public async Task<bool> RegisterService(ServiceRegistrationInfo service)
        {
            return await _client.RegisterService(service);
        }
        [HttpPut]
        public async Task<bool> DeregisterService(string serviceId)
        {
            return await _client.DeregisterService(serviceId);
        }
        [HttpGet]
        public Dictionary<string, ServiceInfo> GetAllServices()
        {
            return  _client.GetAllServices();
        }
    }
}
