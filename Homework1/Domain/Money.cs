namespace Fuse8.BackendInternship.Domain;

/// <summary>
/// Модель для хранения денег
/// </summary>
public class Money
{
	public Money(int rubles, int kopeks)
		: this(false, rubles, kopeks)
	{
	}

	public Money(bool isNegative, int rubles, int kopeks)
	{
		if(rubles < 0 ||  kopeks < 0|| kopeks >= 100 || (isNegative && rubles==0 && kopeks==0))
		{
			throw new ArgumentException();
		}
		IsNegative = isNegative;
		Rubles = rubles;
		Kopeks = kopeks;
		SimpleAmount = rubles * 100 + kopeks;
		if (IsNegative)
		{
			SimpleAmount *=-1;
		}
	}
	private Money(long simpleAmount)
	{
        Rubles = (int)(simpleAmount / 100);
        Kopeks = (int)(simpleAmount % 100);
        SimpleAmount = simpleAmount;
        if (simpleAmount < 0)
		{
			IsNegative = true;
			Rubles *= -1;
			Kopeks *= -1;
		}
		else
		{
			IsNegative = false;
		}
	}

	/// <summary>
	/// Отрицательное значение
	/// </summary>
	public bool IsNegative { get; }

	/// <summary>
	/// Число рублей
	/// </summary>
	public int Rubles { get; }

	/// <summary>
	/// Количество копеек
	/// </summary>
	public int Kopeks { get; }
	private long SimpleAmount { get; }
	public static Money operator +(Money a, Money b)
	{
		return new Money(a.SimpleAmount + b.SimpleAmount);
	}
    public static Money operator -(Money a, Money b)
    {
        return new Money(a.SimpleAmount - b.SimpleAmount);
    }
    public static bool operator <(Money a, Money b)
    {
        return a.SimpleAmount < b.SimpleAmount;
    }
    public static bool operator <=(Money a, Money b)
    {
        return a.SimpleAmount <= b.SimpleAmount;
    }
    public static bool operator >=(Money a, Money b)
    {
        return a.SimpleAmount >= b.SimpleAmount;
    }
    public static bool operator >(Money a, Money b)
    {
        return a.SimpleAmount > b.SimpleAmount;
    }
    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
			return false;
        }
        var comparing = (Money)obj;
		return (IsNegative == comparing.IsNegative && Rubles == comparing.Rubles && Kopeks == comparing.Kopeks);
    }
    public override string ToString()
    {
		return $"IsNegative = {IsNegative}, Rubles = {Rubles}, Kopeks = {Kopeks}";
    }
    public override int GetHashCode()
    {
        return SimpleAmount.GetHashCode();
    }
}