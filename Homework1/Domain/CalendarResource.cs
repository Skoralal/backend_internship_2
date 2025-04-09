﻿namespace Fuse8.BackendInternship.Domain;

/// <summary>
/// Значения ресурсов для календаря
/// </summary>
public class CalendarResource
{
	public static readonly CalendarResource Instance = new();

	public static readonly string January;
	public static readonly string February;

	private static readonly string[] MonthNames;

	static CalendarResource()
	{
		MonthNames = new[]
		{
			"Январь",
			"Февраль",
			"Март",
			"Апрель",
			"Май",
			"Июнь",
			"Июль",
			"Август",
			"Сентябрь",
			"Октябрь",
			"Ноябрь",
			"Декабрь",
		};
        January = GetMonthByNumber(0);
        February = GetMonthByNumber(1);
    }

	private static string GetMonthByNumber(int number)
		=> MonthNames[number];

	// ToDo: реализовать индексатор для получения названия месяца по енаму Month
	public string this[Month month]
	{
		get 
		{
			try
			{
                return MonthNames[(int)month];
            }
			catch
			{
				throw new ArgumentOutOfRangeException(nameof(month));
			}

        }
	}
}

public enum Month
{
	January,
	February,
	March,
	April,
	May,
	June,
	July,
	August,
	September,
	October,
	November,
	December,
}