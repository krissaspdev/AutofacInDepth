using System;
using System.IO;
using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;

namespace TypeInterceptors
{
    public class CallLogger: IInterceptor // from Castle DynamicProxy
    {
        private TextWriter output;

        public CallLogger(TextWriter output)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            output.WriteLine("Calling method {0} with args {1}",
                methodName,
                string.Join(",",
                    invocation.Arguments.Select(a => (a ?? "").ToString())
                )
            );
            invocation.Proceed();
            output.WriteLine("Done calling {0}, result was {1}",
                methodName, invocation.ReturnValue
            );
        }
    }

    public interface IAudit
    {
        int Start(DateTime repotrDate);
    }

    [Intercept(typeof(CallLogger))] // We must decorate this class
    public class Audit : IAudit
    {
        public virtual int Start(DateTime reportDate) // method must be virtual
        {
            Console.WriteLine($"Starting report on {reportDate}");
            return 42;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.Register(a => new CallLogger(Console.Out))
                .As<IInterceptor>()
                .AsSelf();

            containerBuilder.RegisterType<Audit>()
                .EnableClassInterceptors();

            using (var container = containerBuilder.Build())
            {
                container.Resolve<Audit>().Start(DateTime.Now);
            }
        }
    }
}
