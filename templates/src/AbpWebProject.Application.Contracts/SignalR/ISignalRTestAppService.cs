using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpWebProject.Application.Contracts.SignalR
{
    public interface ISignalRTestAppService : IApplicationService, ITransientDependency
    {
        Task SendHelloAsync();

        Task SendHelloToUserAsync();
    }
}
