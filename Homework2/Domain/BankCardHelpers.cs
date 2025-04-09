using System.Reflection;

namespace Fuse8.BackendInternship.Domain;

public static class BankCardHelpers
{
	private static readonly string _covertfieldName = "_number";
	/// <summary>
	/// Получает номер карты без маски
	/// </summary>
	/// <param name="card">Банковская карта</param>
	/// <returns>Номер карты без маски</returns>
	public static string GetUnmaskedCardNumber(BankCard card)
	{
        // ToDo: С помощью рефлексии получить номер карты без маски
		if(card == null) throw new ArgumentNullException(nameof(card), message:$"supplied {nameof(BankCard)} is null");

        var field = typeof(BankCard).GetField(_covertfieldName, BindingFlags.NonPublic | BindingFlags.Instance);
		if(field == null) throw new ArgumentNullException(nameof(field), message: $"no '{_covertfieldName}' field found in the {nameof(BankCard)}");
		var unmaskedNumber = field.GetValue(card);
		if(unmaskedNumber == null) throw new ArgumentNullException(nameof(unmaskedNumber), message:$"supposed {_covertfieldName} field returned null from the {nameof(BankCard)}");
		if (unmaskedNumber.GetType() != typeof(string)) throw new ArgumentException($"{nameof(unmaskedNumber)} awaited {typeof(string)} recieved {unmaskedNumber.GetType()}");
		return unmaskedNumber.ToString()!;
	}
}