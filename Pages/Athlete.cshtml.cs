using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FantasySports.Models;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using FantasySports.Data;

namespace FantasySports.Pages
{
    public class AthleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public AthleteModel(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public List<Athlete> Athlete { get; set; } = new List<Athlete>();

        public async Task OnGetAsync()
        {
            // Fetch athletes where TeamId is NULL
            Athlete = await _context.Athletes
                .ToListAsync();
        }
    }
}