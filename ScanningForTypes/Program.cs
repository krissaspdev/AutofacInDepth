using System;
using System.Reflection;
using Autofac;

namespace ScanningForTypes
{
    public interface ILog
    {
        void Write(string message);
    }

    public interface IConsole
    {

    }

    public class ConsoleLog : ILog, IConsole
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

    public class Engine
    {
        private ILog log;
        private int id;

        public Engine(ILog log)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.id = new Random().Next();
        }

        public void Ahead(int power)
        {
            log.Write($"Engine [{id}] ahead {power}");
        }
    }

    public class Car
    {
        private Engine engine;
        private ILog log;

        public Car(Engine engine, ILog log)
        {
            this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }


        public void Go()
        {
            engine.Ahead(100);
            log.Write("Car going forward...");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // getting assembly
            //var assembly = Assembly.GetExecutingAssembly();
            // or better
            var assembly = typeof(Car).Assembly;

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(assembly)
                .Where(t => t.Name.EndsWith("Log"))
                .Except<SmsLog>()
                .Except<ConsoleLog>(c => c.As<ILog>().SingleInstance())
                .AsSelf();



            //grab all the login clases except smslog
            //and register them to provide a service
            //which corresponds to the first interface they implement
            builder.RegisterAssemblyTypes(assembly)
                .Except<SmsLog>()
                .Where(t => t.Name.EndsWith("Log"))
                .As(t => t.GetInterfaces()[0]); // get the first interface
        }
    }

}
