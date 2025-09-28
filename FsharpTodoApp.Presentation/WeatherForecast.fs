namespace FsharpTodoApp.Presentation

open System
open FsharpTodoApp.Domain.Common.ValueObjects

type WeatherForecast =
    { Date: DateTime
      TemperatureC: int
      Summary: string }

    member this.TemperatureF = 32.0 + (float this.TemperatureC / 0.5556)
