using DJI.Contracts.Enums;

namespace DJI.Contracts.Rqs;

public record PeriodRq
{
    public PeriodPresetEnum Preset { get; set; } = PeriodPresetEnum.Last30Days;

    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }
}
