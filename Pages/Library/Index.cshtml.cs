﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sms.Data;
using sms.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace sms.Pages.Library
{
    [Authorize(Roles = "Адміністратор, Бібліотекар")]
    public class IndexModel : PageModel
    {
        private readonly sms.Data.ApplicationDbContext _context;
        private readonly IConfiguration Configuration;

        public IndexModel(sms.Data.ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        //public IList<Book> Book { get;set; }
        public string NameSort { get; set; }
        public string GradeSort { get; set; }
        public string YearSort { get; set; }
        public string QtySort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public PaginatedList<Book> Book { get; set; }
        public string selectedGrade;
        public List<Grade> gradeList { get; set; }

        public List<SelectListItem> Grades { get; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "0", Text = "Всі класи" },
            new SelectListItem { Value = "1", Text = "1 кл." },
            new SelectListItem { Value = "2", Text = "2 кл." },
            new SelectListItem { Value = "3", Text = "3 кл." },
            new SelectListItem { Value = "4", Text = "4 кл." },
            new SelectListItem { Value = "5", Text = "5 кл." },
            new SelectListItem { Value = "6", Text = "6 кл." },
            new SelectListItem { Value = "7", Text = "7 кл." },
            new SelectListItem { Value = "8", Text = "8 кл." },
            new SelectListItem { Value = "9", Text = "9 кл." },
            new SelectListItem { Value = "10", Text = "10 кл." },
            new SelectListItem { Value = "11", Text = "11 кл." }
        };
        public async Task OnGetAsync(string grade, string sortOrder,
            string currentFilter, string searchString, int? pageIndex)
        {
            gradeList = await _context.Grades.ToListAsync();

            selectedGrade = String.IsNullOrEmpty(grade) ? "0" : grade;
            selectedGrade = grade;
            
            CurrentSort = sortOrder;
            GradeSort = String.IsNullOrEmpty(sortOrder) ? "grade_desc" : "";
            NameSort = sortOrder == "name" ? "name_desc" : "name";
            YearSort = sortOrder == "year" ? "year_desc" : "year";
            QtySort = sortOrder == "qty" ? "qty_desc" : "qty";
            
            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            } 

            CurrentFilter = searchString;

            IQueryable<Book> booksIQ = _context.Books;

            if (!String.IsNullOrEmpty(searchString))
            {
                booksIQ = booksIQ.Where(s => s.Name.Contains(searchString)
                                       || s.Author.Contains(searchString)
                                       || s.PublishingHouse.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "grade_desc":
                    booksIQ = booksIQ.OrderByDescending(s => s.Grade);
                    break;
                case "name":
                    booksIQ = booksIQ.OrderBy(s => s.Name);
                    break;
                case "name_desc":
                    booksIQ = booksIQ.OrderByDescending(s => s.Name);
                    break;
                case "year":
                    booksIQ = booksIQ.OrderBy(s => s.Year);
                    break;
                case "year_desc":
                    booksIQ = booksIQ.OrderByDescending(s => s.Year);
                    break;
                case "qty":
                    booksIQ = booksIQ.OrderBy(s => s.Qty);
                    break;
                case "qty_desc":
                    booksIQ = booksIQ.OrderByDescending(s => s.Qty);
                    break;
                default:
                    booksIQ = booksIQ.OrderBy(s => s.Grade);
                    break;
            }

            if (!String.IsNullOrEmpty(selectedGrade) && Convert.ToInt32(selectedGrade) > 0)
            {
                booksIQ = booksIQ.Where(b => b.Grade == Convert.ToInt32(selectedGrade));
            }

            var pageSize = Configuration.GetValue("PageSize", 10);
            Book = await PaginatedList<Book>.CreateAsync(
                booksIQ.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}