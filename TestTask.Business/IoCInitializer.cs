using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using TestTask.Business.Interfaces;

namespace TestTask.Business
{
    public class IoCInitializer : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IProcessor>().To<Processor>();
            this.Bind<IReader>().To<Reader>();
            this.Bind<IWriter>().To<Writer>();
        }
    }
}
