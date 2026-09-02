namespace DJI.Core.Constants;

public static class ErrorMessages
{
    public const string CustomPeriodRequiresBothBounds = "Для произвольного периода нужны обе границы: from и to.";

    public const string PeriodStartAfterEnd = "Дата начала периода позже даты окончания.";

    public const string PeriodTooLongFormat = "Диапазон не должен превышать {0} дней, запрошено {1}.";

    public const string UnknownPeriodPresetFormat = "Неизвестный пресет периода: {0}.";

    public const string ConnectionStringMissingFormat = "Не задана строка подключения ConnectionStrings:{0}.";

    public const string BadRequestTitle = "Некорректный запрос";

    public const string ServerErrorTitle = "Внутренняя ошибка сервера";

    public const string ServerErrorDetail = "Запрос не удалось обработать. Подробности — в логах сервиса.";
}
