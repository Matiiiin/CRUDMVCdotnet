﻿using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;

namespace Repositories;

public class CountriesRepository(ApplicationDbContext db)  : ICountriesRepository
{
    private readonly ApplicationDbContext _db = db;

    public async Task<Country> AddCountry(Country country)
    {
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();
        return country;
    }

    public async Task<Country?> GetCountryByCountryID(Guid? countryID)
    {
        return await _db.Countries.FirstOrDefaultAsync(c => c.CountryID == countryID);
    }

    public async Task<Country?> GetCountryByCountryName(string countryName)
    {
        return await _db.Countries.FirstOrDefaultAsync(c => c.CountryName == countryName);
    }

    public async Task<List<Country>> GetAllCountries()
    {
        return await _db.Countries.ToListAsync();
    }
}