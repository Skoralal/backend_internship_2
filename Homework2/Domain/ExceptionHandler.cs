using System.Net;

namespace Fuse8.BackendInternship.Domain;

public static class ExceptionHandler
{
	/// <summary>
	/// Обрабатывает исключение, которое может возникнуть при выполнении <paramref name="action"/>
	/// </summary>
	/// <param name="action">Действие, которое может породить исключение</param>
	/// <returns>Сообщение об ошибке</returns>
	public static string? Handle(Action action)
	{
		// ToDo: Реализовать обработку исключений
		try
		{ 
			action();
        }
		catch (MoneyException moneyEx)
		{
			return moneyEx.Message;
		}
		catch (HttpRequestException httpEx) when (httpEx.StatusCode == HttpStatusCode.NotFound)
		{
			return "Ресурс не найден";
		}
        catch (HttpRequestException httpEx)
        {
			return httpEx.StatusCode.ToString();
        }
        catch
		{
			return "Произошла непредвиденная ошибка";
		}

        return null;
	}
}

public class MoneyException : Exception
{
	public MoneyException()
	{
	}

	public MoneyException(string? message)
		: base(message)
	{
	}
}

public class NotValidKopekCountException : MoneyException
{

    public NotValidKopekCountException()
        : base("Количество копеек должно быть больше 0 и меньше 99")
    {
    }
}

public class NegativeRubleCountException : MoneyException
{
    public NegativeRubleCountException()
        : base("Число рублей не может быть отрицательным")
    {
    }
}