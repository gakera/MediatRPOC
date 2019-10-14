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

                    AddPostProcessorsForRequestTypeMarker(services, typeof(IShouldExecuteTask));
                    //AddPostProcessorsForRequestType(services, typeof(IShouldAddAuditLog));
                    AddPostProcessorsForRequestTypeMarker(services, typeof(IShouldMeasure));

                    AddPostProcessorsForResponseTypeMarker(services, typeof(IShouldAddAuditLog));



                });

        private static void AddPostProcessorsForRequestTypeMarker(IServiceCollection services, Type target)
        {
            var types = typeof(Program).GetTypeInfo().Assembly.GetExportedTypes().Distinct();
            var requestResponses = types.Where(x => x.GetInterfaces().Contains(target))
                .Select(x => new
                {
                    Request = x,
                    Response = x.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>)).First().GetGenericArguments().First()
                }).ToList();

            var postProcessors = types.Where(x => x.GetInterfaces()
                .Any(x => x.Name.Contains("IRequestPostProcessor")
                    && x.GetGenericArguments().Any(x => x.Name.Contains(target.Name)))).ToList();

            requestResponses.ForEach(p => postProcessors.ForEach(x => services.AddTransient(typeof(IRequestPostProcessor<,>)
                .MakeGenericType(p.Request, p.Response), x)));
        }

        private static void AddPostProcessorsForResponseTypeMarker(IServiceCollection services, Type target)
        {
            var types = typeof(Program).GetTypeInfo().Assembly.GetExportedTypes().Distinct();

            var requestResponses = types.Where(x => x.GetInterfaces().Contains(target))
                .Select(x => new
                {
                    Request = types.First(y => y.GetInterfaces().Any(z => z.IsGenericType && z.GetGenericTypeDefinition() == typeof(IRequest<>) 
                        && z.GetGenericArguments().FirstOrDefault(r => r == x)?.GetInterfaces().Contains(target) == true)),
                    Response = x
                }).ToList();

            var postProcessors = types.Where(x => x.GetInterfaces()
                .Any(x => x.Name.Contains("IRequestPostProcessor")
                    && x.GetGenericArguments().Any(x => x.Name.Contains(target.Name)))).ToList();

            requestResponses.ForEach(p => postProcessors.ForEach(x => services.AddTransient(typeof(IRequestPostProcessor<,>)
                .MakeGenericType(p.Request, p.Response), x)));
        }
    }
}