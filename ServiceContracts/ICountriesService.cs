﻿using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountriesService
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
        Task<List<CountryResponse>> GetAllCountries();
        Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);
    }
}
