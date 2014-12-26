using System.Configuration;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using FileNotes.Web.DataAccess;
using FileNotes.Web.Models.EntryFileModel;
using FileNotes.Web.Models.NoteModel;

namespace FileNotes.Web
{
    public static class DependencyConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<WebModule>();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
        }
    }

    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FnContext>()
                   .WithParameter("connectionString", ConfigurationManager.ConnectionStrings["filenotes"].ConnectionString)
                   .InstancePerRequest();

            builder.RegisterType<NoteRepository>().As<INoteRepository>().InstancePerRequest();
            builder.RegisterType<NoteFacade>().InstancePerRequest();
            builder.RegisterType<EntryFileRepository>().As<IEntryFileRepository>().InstancePerRequest();
            builder.RegisterType<EntryFileFacade>().InstancePerRequest();
            builder.RegisterControllers(GetType().Assembly); 
        }
    }
}