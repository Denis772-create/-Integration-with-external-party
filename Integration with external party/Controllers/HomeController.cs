using Integration_with_external_party.Infrastructure;
using Integration_with_external_party.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Integration_with_external_party.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var employees = _context.Employees.OrderBy(e => e.LastName).ToList();
        return View(employees);
    }

    [HttpPost("import")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return Content("File not selected or is empty.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;  

        using var reader = new StreamReader(memoryStream);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null, 
            HeaderValidated = null,  
            PrepareHeaderForMatch = header => header.Header.ToLower(),  
        });
        try
        {
            var records = csv.GetRecords<Employee>();
            await _context.Employees.AddRangeAsync(records);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        catch (Exception ex)
        {
            return BadRequest($"An error occurred: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Employee employee)
    {
        _context.Update(employee);
        await _context.SaveChangesAsync();
        return Json(employee);
    }
}