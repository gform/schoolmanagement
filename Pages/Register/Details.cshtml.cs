﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sms.Data;
using sms.Models;

namespace sms.Pages.Register
{
    [Authorize(Roles = "Адміністратор, Вчитель")]
    public class DetailsModel : PageModel
    {
        private readonly sms.Data.ApplicationDbContext _context;

        public DetailsModel(sms.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Gradebook Gradebook { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Gradebook = await _context.Gradebooks
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Include(g => g.Teacher).FirstOrDefaultAsync(m => m.Id == id);

            if (Gradebook == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}