using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.Indexed;

namespace KeyedServiceLookup
{
    public interface ILog
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }

    public class EmailLog : ILog
    {
        private const string adminEmail = "admin@foo.com";
        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {adminEmail} : {message}");
        }
    }

    public class SmsLog : ILog
    {
        private string phoneNumber;

        public SmsLog(string phoneNumber)
        {
            this.phoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
        }

        public void Write(string message)
        {
            Console.WriteLine($"SMS to {phoneNumber} : {message}");
        }
    }

    public class Reporting
    {
        private IIndex<string, ILog> logs;

        public Reporting(IIndex<string, ILog> allLogs)
        {
            this.logs = allLogs ?? throw new ArgumentNullException(nameof(allLogs));
        }

        public void Report()
        {

            logs["sms"].Write("Starting report output");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleLog>().Keyed<ILog>("cmd");
            builder.Register((IComponentContext c) => new SmsLog("+1234546")).Keyed<ILog>("sms");
            builder.RegisterType<Reporting>();

            using (var container = builder.Build())
            {
                container.Resolve<Reporting>().Report();
            }
        }
    }
}
