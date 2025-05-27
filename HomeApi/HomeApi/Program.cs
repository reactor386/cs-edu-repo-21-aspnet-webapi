//-
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

using HomeApi.Tools;
using HomeApi.Configuration;
using HomeApi.Contracts.Validation;
using HomeApi.Data;
using HomeApi.Data.Repos;


namespace HomeApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        // Загрузка конфигурации из файла Json
        builder.Configuration.AddJsonFile("HomeOptions.json");


        // Подключаем автомаппинг
        var mapperConfig = new MapperConfiguration(v =>
        {
            v.AddProfile(new MappingProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        builder.Services.AddSingleton(mapper);


        // регистрация сервиса репозитория для взаимодействия с базой данных
        builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();
        builder.Services.AddSingleton<IRoomRepository, RoomRepository>();

        // получаем строку подключения из файла конфигурации
        string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
        // обновляем публичные значения реальными значениями из приватной области
        connection = ConnectionTools.GetConnectionString(connection);

        // добавляем контекст ApplicationContext в качестве сервиса в приложение
        builder.Services.AddDbContext<HomeApiContext>(options => options.UseSqlServer(connection), ServiceLifetime.Singleton);


        // Подключаем валидацию
        builder.Services.AddValidatorsFromAssemblyContaining<AddDeviceRequestValidator>();

        // добавляем новый сервис, загружаем содержимое целиком
        builder.Services.Configure<HomeOptions>(builder.Configuration);

        // загружаем только секцию адрес (вложенный Json-объект) 
        builder.Services.Configure<Address>(builder.Configuration.GetSection("Address"));


        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        // поддержка автоматической генерации документации WebApi с использованием Swagger
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            
            // app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
