using System;
using Autofac;

namespace RegistrationWithParameters
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
            var builder = new ContainerBuilder();

            // registration by named parameter
            //      builder.RegisterType<SMSLog>()
            //        .As<ILog>()
            //        .WithParameter("phoneNumber", "+12345678");

            // registration by typed parameter
            //      builder.RegisterType<SMSLog>()
            //        .As<ILog>()
            //        .WithParameter(new TypedParameter(typeof(string), "+12345678"));

            // resolved parameter
            //      builder.RegisterType<SMSLog>()
            //        .As<ILog>()
            //        .WithParameter(
            //          new ResolvedParameter(
            //            // predicate (filter how to find parameter)
            //            (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
            //            // value accessor - how to get the value of parameter (we na use component context)
            //            (pi, ctx) => "+12345678"
            //          )
            //        );
            //
            //

            // specyfing parameter on resolve time
            Random random = new Random();
            builder.Register((c, p) => new SmsLog(p.Named<string>("phoneNumber")))
                .As<ILog>();

            Console.WriteLine("About to build container...");
            var container = builder.Build();

            var log = container.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));
            log.Write("Testing");
        }
    }
}
