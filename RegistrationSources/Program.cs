using System;
using Autofac;
using Autofac.Features.ResolveAnything;

namespace RegistrationSources
{
    public interface ICanSpeak
    {
        void Speak();
    }

    public class Person : ICanSpeak
    {
        public void Speak()
        {
            Console.WriteLine("Hello");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource()); // resisters everything

            using (var c = builder.Build())
            {
                c.Resolve<Person>().Speak();
            }
        }
    }
}
