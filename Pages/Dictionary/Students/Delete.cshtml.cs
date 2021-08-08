﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sms;
using sms.Data;

namespace sms.Pages.Students
{
    [Authorize(Roles = "Адміністратор")]
    public class DeleteModel : PageModel
    {
        private readonly sms.Data.ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DeleteModel(sms.Data.ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public Student Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Students
                .Include(s => s.Grade).FirstOrDefaultAsync(m => m.Id == id);

            if (Student == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(Student.Gender)) Student.Gender = "Не вказано";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Student = await _context.Students.FindAsync(id);

            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, @"images/avatars"); //webHost adds 'wwwroot'
            var oldFile = Student.ProfilePicture;
            var fileToDelete = string.Empty;
            if (!string.IsNullOrEmpty(oldFile))
            {
                fileToDelete = Path.Combine(uploadsFolder, oldFile);
            }

            if (Student != null)
            {
                _context.Students.Remove(Student);
                await _context.SaveChangesAsync();

                if (System.IO.File.Exists(fileToDelete))
                {
                    System.IO.File.Delete(fileToDelete);
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
