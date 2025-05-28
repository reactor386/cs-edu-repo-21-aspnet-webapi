//-
using System;


namespace HomeApi.Configuration;

/// <summary>
/// Информация о нашем доме
/// </summary>
public class HomeOptions
{
    public int FloorAmount { get; set; }
    public string Telephone { get; set; } = null!;
    public Heating Heating { get; set; }
    public int CurrentVolts { get; set; }
    public bool GasConnected { get; set; }
    public int Area { get; set; }
    public Material Material { get; set; }
    public Address Address { get; set; } = null!;
}
