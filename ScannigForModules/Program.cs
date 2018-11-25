using System;
using System.Runtime.CompilerServices;
using Autofac;

namespace ScannigForModules
{
    public class Parent
    {
        public override string ToString()
        {
            return "I'm your father";
        }
    }

    public class Child
    {
        public string Name { get; set; }
        public Parent Parent { get; set; }

        public void SetParent(Parent parent)
        {
            Parent = parent;
        }
    }

    public class ParentChildModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Parent>();
            builder.Register(c => new Child {Parent = c.Resolve<Parent>()});
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            //1. method
            //builder.RegisterAssemblyModules(typeof(Program).Assembly);

            //2. method
            builder.RegisterAssemblyModules<ParentChildModule>(typeof(Program).Assembly);
            var container = builder.Build();

            Console.WriteLine(container.Resolve<Child>().Parent);
        }
    }

}
