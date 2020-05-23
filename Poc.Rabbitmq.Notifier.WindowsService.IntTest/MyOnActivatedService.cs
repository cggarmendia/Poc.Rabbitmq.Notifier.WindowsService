using System.Diagnostics;

namespace Poc.Rabbitmq.Notifier.WindowsService.IntTest
{
    [Vueling.Extensions.Library.DI.RegisterOnActivated]
    public class MyOnActivatedService
    {
        [Vueling.Extensions.Library.DI.RegisterActionOnActivated]
        public void PrintTrace()
        {
            Trace.TraceInformation("Invoked MyStatefulService.PrintTrace");
        }
    }
}
