﻿using Common.Models;
using InternalApi.Models;

namespace InternalApi.Contracts;

interface ICachedCurrencyAPI
{
    /// <summary>
    /// Получает текущий курс
    /// </summary>
    /// <param name="currencyType">Валюта, для которой необходимо получить курс</param>
    /// <param name="baseCurrencyType"></param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Текущий курс</returns>
    Task<CurrencyDTO> GetCurrentCurrencyAsync(CurrencyType currencyType, CurrencyType baseCurrencyType, CancellationToken cancellationToken);

    /// <summary>
    /// Получает курс валюты, актуальный на <paramref name="date"/>
    /// </summary>
    /// <param name="currencyType">Валюта, для которой необходимо получить курс</param>
    /// <param name="baseCurrencyType"></param>
    /// <param name="date">Дата, на которую нужно получить курс валют</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>Курс на дату</returns>
    Task<CurrencyDTO> GetCurrencyOnDateAsync(CurrencyType currencyType, CurrencyType baseCurrencyType, DateOnly date, CancellationToken cancellationToken);
}

// Данные модели использовать не обязательно, можно реализовать свои


