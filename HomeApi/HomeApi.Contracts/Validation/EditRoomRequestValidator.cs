//-
using System;
using System.Collections.Generic;
using System.Linq;

using FluentValidation;

using HomeApi.Contracts.Models.Rooms;


namespace HomeApi.Contracts.Validation;

/// <summary>
/// Класс-валидатор запросов обновления комнаты
/// </summary>
public class EditRoomRequestValidator : AbstractValidator<EditRoomRequest>
{
    /// <summary>
    /// Метод - конструктор, устанавливающий правила
    /// </summary>
    public EditRoomRequestValidator()
    {
        RuleFor(x => x.NewName).NotEmpty().Must(BeSupported)
            .WithMessage($"Please choose one of the following names: {string.Join(", ", Values.ValidRooms)}");
        RuleFor(x => x.NewArea).NotEmpty().NotEqual(0);
    }
    
    /// <summary>
    ///  Метод кастомной валидации для свойства name
    /// </summary>
    private bool BeSupported(string? name)
    {
        return Values.ValidRooms.Any(e => e == name);
    }
}
