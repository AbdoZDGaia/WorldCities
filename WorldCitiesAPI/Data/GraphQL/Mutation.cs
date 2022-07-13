using AutoMapper;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Data.GraphQL
{
    public class Mutation
    {
        /// <summary>
        /// Add a new City
        /// </summary>
        [Serial]
        [Authorize(Roles = new[] { "Registered" })]
        public async Task<City> AddCity([Service] ApplicationDbContext context,
            [Service] IMapper mapper,
            CityDTO cityDTO)
        {
            var city = mapper.Map<City>(cityDTO);
            context.Cities.Add(city);
            await context.SaveChangesAsync();
            return city;
        }

        /// <summary>
        /// Update an existing City
        /// </summary>
        [Serial]
        [Authorize(Roles = new[] { "RegisteredUser" })]
        public async Task<City> UpdateCity([Service] ApplicationDbContext context,
            [Service] IMapper mapper,
            CityDTO cityDTO)
        {
            var city = await context.Cities
            .Where(c => c.Id == cityDTO.Id)
            .FirstOrDefaultAsync();
            if (city == null)
                // todo: handle errors
                throw new NotSupportedException();

            mapper.Map(cityDTO, city);
            await context.SaveChangesAsync();
            return city;
        }

        /// <summary>
        /// Delete a City
        /// </summary>
        [Serial]
        [Authorize(Roles = new[] { "Administrator" })]
        public async Task DeleteCity(
        [Service] ApplicationDbContext context, int id)
        {
            var city = await context.Cities
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
            if (city != null)
            {
                context.Cities.Remove(city);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Add a new Country
        /// </summary>
        [Serial]
        [Authorize(Roles = new[] { "RegisteredUser" })]
        public async Task<Country> AddCountry([Service] ApplicationDbContext context,
            [Service] IMapper mapper,
            CountryDTO countryDTO)
        {
            var country = mapper.Map<Country>(countryDTO);
            context.Countries.Add(country);
            await context.SaveChangesAsync();
            return country;
        }

        /// <summary>
        /// Update an existing Country
        /// </summary>
        [Serial]
        [Authorize(Roles = new[] { "RegisteredUser" })]
        public async Task<Country> UpdateCountry([Service] ApplicationDbContext context,
            [Service] IMapper mapper,
            CountryDTO countryDTO)
        {
            var country = await context.Countries
            .Where(c => c.Id == countryDTO.Id)
            .FirstOrDefaultAsync();
            if (country == null)
                // todo: handle errors
                throw new NotSupportedException();

            mapper.Map(countryDTO, country);
            await context.SaveChangesAsync();
            return country;
        }

        /// <summary>
        /// Delete a Country
        /// </summary>
        [Serial]
        [Authorize(Roles = new[] { "Administrator" })]
        public async Task DeleteCountry([Service] ApplicationDbContext context, int id)
        {
            var country = await context.Countries
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();
            if (country != null)
            {
                context.Countries.Remove(country);
                await context.SaveChangesAsync();
            }
        }
    }
}
