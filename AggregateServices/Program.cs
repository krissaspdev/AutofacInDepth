using System;
using Autofac;
using Autofac.Extras.AggregateService;

namespace AggregateServices
{
    public interface IService1 { }
    public interface IService2 { }
    public interface IService3 { }
    public interface IService4 { }

    public interface IService5
    {
        void WriteName();
    }

    public class Class1 : IService1 { }
    public class Class2 : IService2 { }
    public class Class3 : IService3 { }
    public class Class4 : IService4 { }

    public class Class5 : IService5
    {
        private string name;

        public Class5(string name)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void WriteName()
        {
            Console.WriteLine(name);
        }
    }


    public interface IMyAggregateService
    {
        IService1 Service1 { get; }
        IService2 Service2 { get; }
        IService3 Service3 { get; }
        IService4 Service4 { get; }

        IService5 GetFifthService(string name); // last service is exposed as a method (because it requires a parameter)
        // dynamic proxy is able to genarate this
    }

    public class Consumer
    {
        public IMyAggregateService AllServices;

        public Consumer(IMyAggregateService allServices) // using only one service, not 4 !!!!!
        {
            AllServices = allServices ?? throw new ArgumentNullException(nameof(allServices));
        }
    }

    class Program
    {
        static void Main(string[] args) // You have to install Autofac.Extras.AggregateService to do this
        {
            var builder = new ContainerBuilder();
            builder.RegisterAggregateService<IMyAggregateService>();

            builder.RegisterAssemblyTypes(typeof(Program).Assembly) //registering by filtering assembly
                .Where(t => t.Name.StartsWith("Class"))
                .AsImplementedInterfaces();

            builder.RegisterType<Consumer>();

            using (var container = builder.Build())
            {
                var consumer = container.Resolve<Consumer>();
                Console.WriteLine(consumer.AllServices.Service3.GetType().Name);

                consumer.AllServices.GetFifthService("Test name").WriteName();
            }
        }
    }
}
