//-
using System;
using System.Text;

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using AutoMapper;

using HomeApi.Configuration;
using HomeApi.Contracts.Models.Home;


namespace HomeApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    // Ссылка на объект конфигурации
    private IOptions<HomeOptions> _options;
    private IOptions<Address> _optionsAddress;
    private IMapper _mapper;

    // Инициализация конфигурации при вызове конструктора
    public HomeController(IOptions<HomeOptions> options, IOptions<Address> optionsAddress, IMapper mapper)
    {
        _options = options;
        _optionsAddress = optionsAddress;
        _mapper = mapper;
    }


    /// <summary>
    /// Метод для получения информации о доме
    /// </summary>
    [HttpGet]
    [Route("info")] 
    public IActionResult Info()
    {
        // Получим запрос, смапив конфигурацию на модель запроса
        var infoResponse = _mapper.Map<HomeOptions, InfoResponse>(_options.Value);
        // Вернём ответ
        return StatusCode(200, infoResponse);
    }
}
