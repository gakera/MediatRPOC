using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MediatRPipelinePOCWIP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMediatR(typeof(Program).GetTypeInfo().Assembly);
                    //services.AddTransient<IRequestPostProcessor<CommandRequest, CommandResponse>, CommandRequestPostProcessor>();
                    //services.AddTransient<IRequestPostProcessor<CommandRequest, CommandResponse>, AlsoCommandRequestPostProcessor>();
                    services.AddHostedService<Worker>();

                    AddPostProcessorsForType(services, typeof(IShouldExecuteTask));
                    AddPostProcessorsForType(services, typeof(IShouldAddAuditLog));
                    AddPostProcessorsForType(services, typeof(IShouldMeasure));

                });

        private static void AddPostProcessorsForType(IServiceCollection services, Type target)
        {
            var na = target.Name;
            var newlist = typeof(Program).GetTypeInfo().Assembly.GetExportedTypes().Distinct();
            var taskRe = newlist.Where(x => x.GetInterfaces().Contains(target))
                .Select(x => new
                {
                    Request = x,
                    Response = x.GetInterfaces().Where(x => x.Name.Contains("IRequest")).First().GetGenericArguments().First()
                }).ToList();

            var taskPostProcessors = newlist.Where(x => x.GetInterfaces()
                .Any(x => x.Name.Contains("IRequestPostProcessor")
                    && x.GetGenericArguments().Any(x => x.Name.Contains(target.Name)))).ToList();

            taskRe.ForEach(p => taskPostProcessors.ForEach(x => services.AddTransient(typeof(IRequestPostProcessor<,>)
                .MakeGenericType(p.Request, p.Response), x)));
        }
    }
}