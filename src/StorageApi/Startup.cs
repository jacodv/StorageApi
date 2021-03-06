using System;
using AutoMapper;
using FluentValidation.AspNetCore;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Voyager;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Repositories;
using MongoDB.Repositories.Interfaces;
using MongoDB.Repositories.Settings;
using Serilog;
using StorageApi.Data;
using StorageApi.GraphQL;
using StorageApi.Services;

namespace StorageApi
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
      services.Configure<DatabaseSettings>(Configuration.GetSection(nameof(DatabaseSettings)));

      IDatabaseSettings databaseSettings = new DatabaseSettings();
      services.AddSingleton(serviceProvider =>
      {
        databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
        return databaseSettings;
      });
      services.AddAutoMapper(typeof(Startup));
      services.AddScoped<IUserSession>(provider => new UserSession());

      services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

      services.AddControllers()
        .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null)
        .AddFluentValidation(opt =>
        {
          opt.RegisterValidatorsFromAssemblyContaining<Startup>();
          opt.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
        });

      // Register the Swagger generator, defining 1 or more Swagger documents
      services.AddSwaggerGen();

      // Register the GraphQL implementation (HotChocolate)
      services.AddGraphQLServer()
        .AddQueryType<StorageApiQueryType>(d => 
          d.Name("Query"))
        .AddType<StorageUnitType>()
        ;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddSerilog();

      SetupDatabase.Init(serviceProvider).Wait();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseHttpsRedirection();

      // Enable middleware to serve generated Swagger as a JSON endpoint.
      app.UseSwagger();

      // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
      // specifying the Swagger JSON endpoint.
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Storage API V1");
      });


      app.UseRouting();
      app.UseAuthorization();

      app
        .UsePlayground(new PathString("/graphql"), new PathString("/ui/playground"))
        .UseVoyager(new PathString("/graphql"), new PathString("/ui/voyager"));

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapGraphQL();
      });
    }
  }
}
