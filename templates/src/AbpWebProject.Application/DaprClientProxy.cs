using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpWebProject.Application
{
    public class DaprClientProxy<TRemoteService> : IDaprClientProxy<TRemoteService>
    {
        public TRemoteService Service { get; }

        public DaprClientProxy(TRemoteService service)
        {
            Service = service;
        }
    }
}
