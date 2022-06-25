using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Import()
        {
            if (!_env.IsDevelopment())
                throw new SecurityException("Not Allowed");

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var excelPath = Path.Combine(_env.ContentRootPath, "Data/Sources/Worldcities.xlsx");

            using var stream = System.IO.File.OpenRead(excelPath);
            using var excelPackage = new ExcelPackage(stream);

            var workSheet = excelPackage.Workbook.Worksheets[0];
            var nEndRow = workSheet.Dimension.End.Row;

            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;

            var countriesByName = _context.Countries
                .AsNoTracking()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = workSheet
                    .Cells[nRow,
                    1,
                    nRow,
                    workSheet.Dimension.End.Column];

                var countryName = row[nRow, 5].GetValue<string>();
                var iso2 = row[nRow, 6].GetValue<string>();
                var iso3 = row[nRow, 7].GetValue<string>();

                if (countriesByName.ContainsKey(countryName))
                    continue;

                var country = new Country
                {
                    Name = countryName,
                    ISO2 = iso2,
                    ISO3 = iso3,
                };

                await _context.Countries.AddAsync(country);

                countriesByName.Add(countryName, country);

                numberOfCountriesAdded++;
            }

            if (numberOfCountriesAdded > 0)
                await _context.SaveChangesAsync();

            var cities = _context.Cities.AsNoTracking().ToDictionary(x => (Name: x.Name, Lat: x.Lat, Lon: x.Lon, CountryId: x.CountryId));

            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = workSheet.Cells[
                nRow, 1, nRow, workSheet.Dimension.End.Column];
                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();
                var countryId = countriesByName[countryName].Id;
                if (cities.ContainsKey((Name: name, Lat: lat, Lon: lon, CountryId: countryId)))
                    continue;
                var city = new City
                {
                    Name = name,
                    Lat = lat,
                    Lon = lon,
                    CountryId = countryId
                };
                _context.Cities.Add(city);
                numberOfCitiesAdded++;
            }
            if (numberOfCitiesAdded > 0)
                await _context.SaveChangesAsync();

            return new JsonResult(new
            {
                Cities = numberOfCitiesAdded,
                Countries = numberOfCountriesAdded
            });
        }
    }
}
