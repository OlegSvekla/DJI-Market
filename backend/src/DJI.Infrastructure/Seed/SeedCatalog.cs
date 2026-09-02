namespace DJI.Infrastructure.Seed;

internal static class SeedCatalog
{
    internal record ProductSpec(string Name, string Sku, decimal ListPrice, decimal Margin);

    internal record CategorySpec(string Name, ProductSpec[] Products);

    internal static readonly string[] Teams =
    [
        "Корпоративные продажи",
        "Розница",
        "Партнёрская сеть",
        "Онлайн-канал",
    ];

    internal static readonly string[] Positions =
    [
        "Менеджер по продажам",
        "Старший менеджер",
        "Ведущий менеджер",
        "Руководитель группы",
    ];

    internal static readonly string[] AvatarColors =
    [
        "#2563eb", "#7c3aed", "#0891b2", "#059669", "#d97706",
        "#dc2626", "#db2777", "#4f46e5", "#0d9488", "#ca8a04",
    ];

    internal static readonly string[] FirstNames =
    [
        "Алексей", "Мария", "Дмитрий", "Анна", "Сергей", "Екатерина", "Игорь", "Ольга",
        "Павел", "Наталья", "Артём", "Юлия", "Кирилл", "Светлана", "Роман", "Ирина",
        "Максим", "Елена", "Никита", "Татьяна", "Владимир", "Дарья", "Андрей", "Ксения",
    ];

    internal static readonly string[] LastNames =
    [
        "Ковалёв", "Смирнова", "Орлов", "Зайцева", "Морозов", "Лебедева", "Волков", "Соколова",
        "Медведев", "Егорова", "Никитин", "Фомина", "Гусев", "Романова", "Тарасов", "Белова",
        "Шестаков", "Полякова", "Крылов", "Наумова", "Савельев", "Ершова", "Панов", "Демидова",
    ];

    internal static string LastNameFor(int firstNameIndex, int shift)
    {
        var pairs = LastNames.Length / 2;
        var pairIndex = Math.Abs(shift * 7 + 3) % pairs;

        return LastNames[pairIndex * 2 + firstNameIndex % 2];
    }

    internal static readonly string[] CompanyPrefixes =
    [
        "Аэро", "Гео", "Топо", "Агро", "Терра", "Скай", "Орбита", "Вектор",
        "Полёт", "Картон", "Медиа", "Кадр", "Ракурс", "Дельта", "Север", "Сигма",
    ];

    internal static readonly string[] CompanySuffixes =
    [
        "Съёмка", "Скан", "Групп", "Проект", "Сервис", "Технологии", "Лаб", "Строй",
        "Инжиниринг", "Медиа",
    ];

    internal static readonly string[] CompanyForms = ["ООО", "АО", "ИП", "ГК"];

    internal static readonly CategorySpec[] Categories =
    [
        new("Дроны", [
            new("Mavic 4 Pro", "DRN-M4P", 289_000m, 0.14m),
            new("Mavic 4 Pro Fly More Combo", "DRN-M4PC", 359_000m, 0.15m),
            new("Air 3S", "DRN-A3S", 139_000m, 0.16m),
            new("Mini 5 Pro", "DRN-M5P", 94_000m, 0.18m),
            new("Mini 4K", "DRN-M4K", 42_000m, 0.20m),
            new("Avata 2", "DRN-AV2", 109_000m, 0.17m),
            new("Neo", "DRN-NEO", 39_000m, 0.22m),
            new("Matrice 400", "DRN-MT400", 1_290_000m, 0.11m),
            new("Matrice 4E", "DRN-MT4E", 690_000m, 0.12m),
            new("Agras T70P", "DRN-AGT70", 2_450_000m, 0.10m),
        ]),
        new("Экшн-камеры", [
            new("Osmo Action 5 Pro", "CAM-OA5P", 44_900m, 0.24m),
            new("Osmo Action 4", "CAM-OA4", 32_900m, 0.26m),
            new("Osmo Pocket 3", "CAM-OP3", 61_900m, 0.22m),
            new("Osmo Pocket 3 Creator Combo", "CAM-OP3C", 78_900m, 0.23m),
            new("Osmo 360", "CAM-O360", 54_900m, 0.25m),
        ]),
        new("Стабилизаторы", [
            new("RS 4 Pro", "GMB-RS4P", 98_000m, 0.21m),
            new("RS 4", "GMB-RS4", 63_000m, 0.22m),
            new("RS 4 Mini", "GMB-RS4M", 41_000m, 0.24m),
            new("Osmo Mobile 7P", "GMB-OM7P", 15_900m, 0.30m),
            new("Ronin 4D-8K", "GMB-R4D8", 1_150_000m, 0.12m),
        ]),
        new("Питание и зарядка", [
            new("Аккумулятор Intelligent Flight Mavic 4", "PWR-BATM4", 24_900m, 0.32m),
            new("Аккумулятор Air 3S", "PWR-BATA3", 16_900m, 0.33m),
            new("Зарядный хаб Mavic 4", "PWR-HUBM4", 12_400m, 0.35m),
            new("Автомобильное ЗУ 100W", "PWR-CAR100", 6_900m, 0.38m),
            new("Powerbank Osmo 20000", "PWR-PB20", 8_900m, 0.36m),
        ]),
        new("Аксессуары", [
            new("Пропеллеры Mavic 4 (пара)", "ACC-PRPM4", 3_400m, 0.45m),
            new("ND-фильтры Air 3S (набор)", "ACC-NDA3", 9_800m, 0.42m),
            new("Кейс Mavic 4 Pro", "ACC-CASM4", 18_500m, 0.40m),
            new("Микрофон Mic 3 (2 TX + RX)", "ACC-MIC3", 34_900m, 0.28m),
            new("Пульт RC Pro 2", "ACC-RCP2", 89_000m, 0.19m),
            new("Очки Goggles 3", "ACC-GG3", 62_000m, 0.20m),
            new("Карта microSD 512GB", "ACC-SD512", 7_200m, 0.44m),
            new("Посадочная площадка 110 см", "ACC-PAD110", 4_100m, 0.50m),
        ]),
        new("Сервис и обучение", [
            new("Расширенная гарантия Care Refresh 1 год", "SRV-CR1", 21_900m, 0.62m),
            new("Care Refresh 2 года", "SRV-CR2", 34_900m, 0.60m),
            new("Курс пилотирования, базовый", "SRV-EDU1", 29_000m, 0.66m),
            new("Курс аэросъёмки, продвинутый", "SRV-EDU2", 54_000m, 0.64m),
            new("Пусконаладка Agras", "SRV-AGR", 120_000m, 0.55m),
            new("Техобслуживание Matrice", "SRV-MTC", 45_000m, 0.58m),
        ]),
    ];
}
