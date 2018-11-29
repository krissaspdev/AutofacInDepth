using System;
using Autofac;

namespace DelayedLazyInstantiation
{
    public interface ILog: IDisposable
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public ConsoleLog()
        {
            Console.WriteLine($"Console log created at {DateTime.Now.Ticks}");
        }
        public void Write(string message)
        {
            Console.WriteLine(message);
        }

        public void Dispose()
        {
            Console.WriteLine("Console logger no longer required");
        }
    }

    public class EmailLog : ILog
    {
        private const string adminEmail = "admin@foo.com";
        public void Write(string message)
        {
            Console.WriteLine($"Email sent to {adminEmail} : {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Email logger no longer required");
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

        public void Dispose()
        {
            Console.WriteLine("Sms logger no longer required");
        }
    }

    public class Reporting
    {
        private Lazy<ConsoleLog> log;

        public Reporting(Lazy<ConsoleLog> log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            Console.WriteLine("Reporting componen created");
        }

        public void Report()
        {
            Console.WriteLine("Log started");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleLog>();
            builder.RegisterType<Reporting>();

            using (var container = builder.Build())
            {
                container.Resolve<Reporting>().Report();
            }
        }
    }
}
