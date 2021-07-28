using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpWebProject.Application
{
    public interface IDaprClientProxy<out TRemoteService>
    {
        TRemoteService Service { get; }
    }
}
