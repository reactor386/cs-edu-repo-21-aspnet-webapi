//-
using System;


namespace HomeApi.Data.Queries;

/// <summary>
/// Класс для передачи дополнительных параметров при обновлении комнаты
/// </summary>
public class UpdateRoomQuery
{
    public string? NewName { get; }
    public int? NewArea { get; }
    public bool? NewGasConnected { get; }
    public int? NewVoltage { get; }

    public UpdateRoomQuery(
        string? newName = null,
        int? newArea = null,
        bool? newGasConnected = null,
        int? newVoltage = null)
    {
        NewName = newName;
        NewArea = newArea;
        NewGasConnected = newGasConnected;
        NewVoltage = newVoltage;
    }
}
