//-
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using AutoMapper;

using HomeApi.Contracts.Models.Rooms;
using HomeApi.Data.Models;
using HomeApi.Data.Repos;


namespace HomeApi.Controllers;

/// <summary>
/// Контроллер комнат
/// </summary>
[ApiController]
[Route("[controller]")]
public class RoomsController : ControllerBase
{
    private IRoomRepository _repository;
    private IMapper _mapper;
    
    public RoomsController(IRoomRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    //TODO: Задание - добавить метод на получение всех существующих комнат
    
    /// <summary>
    /// Добавление комнаты
    /// </summary>
    [HttpPost] 
    [Route("")] 
    public async Task<IActionResult> Add([FromBody] AddRoomRequest request)
    {
        var existingRoom = await _repository.GetRoomByName(request.Name);
        if (existingRoom == null)
        {
            var newRoom = _mapper.Map<AddRoomRequest, Room>(request);
            await _repository.AddRoom(newRoom);
            return StatusCode(201, $"Комната {request.Name} добавлена!");
        }
        
        return StatusCode(409, $"Ошибка: Комната {request.Name} уже существует.");
    }
}
