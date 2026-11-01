namespace EstiloCorte.Application.Exceptions;

public class ConflictoHorarioException : InvalidOperationException
{
    public ConflictoHorarioException(string message, IReadOnlyList<DateTime> horariosSugeridos)
        : base(message)
    {
        HorariosSugeridos = horariosSugeridos;
    }

    public IReadOnlyList<DateTime> HorariosSugeridos { get; }
}
