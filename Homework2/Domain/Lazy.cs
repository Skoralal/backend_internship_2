namespace Fuse8.BackendInternship.Domain;

/// <summary>
/// Контейнер для значения, с отложенным получением
/// </summary>
public class Lazy<TValue>
{
	// ToDo: Реализовать ленивое получение значение при первом обращении к Value
	private readonly Func<TValue> _initorContainer;
	private static TValue? _lazyVar;
	private bool isInit = false;
	public TValue? Value
	{
		get
		{
			if (!isInit)
			{
				Init();
			}
			return _lazyVar;
		}
	}
	public Lazy(Func<TValue> value)
	{
		_initorContainer = value;
	}
	private void Init()
	{
		_lazyVar = _initorContainer();
		isInit = true;
	}
}