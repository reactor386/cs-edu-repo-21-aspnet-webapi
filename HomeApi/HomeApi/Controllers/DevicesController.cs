//-
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using AutoMapper;

using HomeApi.Configuration;
using HomeApi.Contracts.Models.Devices;
using HomeApi.Data.Models;
using HomeApi.Data.Queries;
using HomeApi.Data.Repos;


namespace HomeApi.Controllers;

/// <summary>
/// Контроллер устройсив
/// </summary>
[ApiController]
[Route("[controller]")]
public class DevicesController : ControllerBase
{
    private readonly ILogger<DevicesController> _logger;
    private readonly IHostEnvironment _env;
    private IOptions<HomeOptions> _options;
    private IMapper _mapper;
    private IDeviceRepository _devices;
    private IRoomRepository _rooms;

    public DevicesController(ILogger<DevicesController> logger, IHostEnvironment env,
        IOptions<HomeOptions> options, IMapper mapper, IDeviceRepository devices, IRoomRepository rooms)
    {
        _logger = logger;
        _env = env;
        _options = options;
        _mapper = mapper;
        _devices = devices;
        _rooms = rooms;
    }


    /// <summary>
    /// Просмотр списка подключенных устройств
    /// </summary>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> GetDevices()
    {
        var devices = await _devices.GetDevices();

        var resp = new GetDevicesResponse
        {
            DeviceAmount = devices.Length,
            Devices = _mapper.Map<Device[], DeviceView[]>(devices)
        };
        
        return StatusCode(200, resp);
    }


    // TODO: Задание: напишите запрос на удаление устройства

    /// <summary>
    /// Удаление устройства
    /// </summary>
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        Device device;
        device = await _devices.GetDeviceById(id);
        if (device != null)
            await _devices.DeleteDevice(device);

        device = await _devices.GetDeviceById(id);
        if (device == null)
        {
            return StatusCode(204);  // No Content
        }
        else
        {
            return StatusCode(500, $"Ошибка удаления устройства {id}");
        }
    }


    /// <summary>
    /// Добавление нового устройства
    /// </summary>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Add(AddDeviceRequest request)
    {
        var room = await _rooms.GetRoomByName(request.RoomLocation);
        if(room == null)
            return StatusCode(400, $"Ошибка: Комната {request.RoomLocation} не подключена. Сначала подключите комнату!");
        
        var device = await _devices.GetDeviceByName(request.Name);
        if(device != null)
            return StatusCode(400, $"Ошибка: Устройство {request.Name} уже существует.");
        
        var newDevice = _mapper.Map<AddDeviceRequest, Device>(request);
        await _devices.SaveDevice(newDevice, room);
        
        return StatusCode(201, $"Устройство {request.Name} добавлено. Идентификатор: {newDevice.Id}");
    }
    
    
    /// <summary>
    /// Обновление существующего устройства
    /// </summary>
    [HttpPatch]
    [Route("{id}")]
    public async Task<IActionResult> Edit(
        [FromRoute] Guid id,
        [FromBody]  EditDeviceRequest request)
    {
        var room = await _rooms.GetRoomByName(request.NewRoom);
        if(room == null)
            return StatusCode(400, $"Ошибка: Комната {request.NewRoom} не подключена. Сначала подключите комнату!");
        
        var device = await _devices.GetDeviceById(id);
        if(device == null)
            return StatusCode(400, $"Ошибка: Устройство с идентификатором {id} не существует.");
        
        var withSameName = await _devices.GetDeviceByName(request.NewName);
        if(withSameName != null)
            return StatusCode(400, $"Ошибка: Устройство с именем {request.NewName} уже подключено. Выберите другое имя!");

        await _devices.UpdateDevice(
            device,
            room,
            new UpdateDeviceQuery(request.NewName, request.NewSerial)
        );

        return StatusCode(200, $"Устройство обновлено! Имя - {device.Name}, Серийный номер - {device.SerialNumber},  Комната подключения - {device.Room.Name}");
    }
}
