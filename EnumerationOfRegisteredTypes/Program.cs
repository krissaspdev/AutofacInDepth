using System;
using System.Collections.Generic;
using System.ComponentModel;
using Autofac;

namespace EnumerationOfRegisteredTypes
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
        private IList<ILog> allLogs;

        public Reporting(IList<ILog> allLogs)
        {
            this.allLogs = allLogs ?? throw new ArgumentNullException(nameof(allLogs));
        }

        public void Report()
        {
            foreach (var log in allLogs)
            {
                log.Write($"Hello, this is {log.GetType().Name}");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleLog>().As<ILog>();
            builder.Register((IComponentContext c) => new SmsLog("+1234546")).As<ILog>();
            builder.RegisterType<Reporting>();

            using (var container = builder.Build())
            {
                container.Resolve<Reporting>().Report();
            }
        }
    }
}
