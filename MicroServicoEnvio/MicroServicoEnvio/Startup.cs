using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicoEnvio.Controllers;
using MicroServicoEnvio.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;

namespace MicroServicoEnvio
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            //Gera um arquivo de log todo dia com a data do dia
            loggerFactory.AddFile("Logs/MicroServicoEnvio.txt");

            StartScheduler();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        //Inicializando a Scheduler
        private void StartScheduler()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = factory.GetScheduler().Result;
            scheduler.Start().Wait();
            ScheduleJobs(scheduler);
        }

        //Criando o Job e a Trigger para a execução recorrente
        private void ScheduleJobs(IScheduler scheduler)
        {
            IJobDetail job = JobBuilder.Create<TimerController>()
                .WithIdentity("timerJob", "timerGroup")
                .Build();

            // Trigger para que o job seja executado imediatamente e repetidamente a cada 5 segundos
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("timerTrigger", "timerGroup")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInSeconds(5)
                 //.WithRepeatCount(100)) //Pode ser utlizado para definir um limite de disparos ao invés do .RepeatForever 
                  .RepeatForever())
              .Build();
            scheduler.ScheduleJob(job, trigger).Wait();
        }
    }
}
