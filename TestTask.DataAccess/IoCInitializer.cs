using Ninject.Modules;
using TestTask.DataAccess.Attributes;
using TestTask.DataAccess.Interfaces;

namespace TestTask.DataAccess
{
    public class IoCInitializer : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IFile>().To<InputFile>().WhenTargetHas<InputFileAttribute>();
            this.Bind<IFile>().To<OutputFile>().WhenTargetHas<OutputFileAttribute>();
        }
    }
}
