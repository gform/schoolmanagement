using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace sms.Pages
{
    [Authorize(Roles = "������������")]
    public class AdminModel : PageModel
    {
        public SelectList RoleNameSL { get; set; }
        private readonly Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _usermanager;
        private readonly IConfiguration Configuration;
        public bool NoRoles { get; set; }

        public AdminModel(sms.Data.ApplicationDbContext context, 
            UserManager<IdentityUser> usermanager, 
            IConfiguration configuration)
        {
            _context = context;
            _usermanager = usermanager;
            Configuration = configuration;
            NoRoles = true;
        }
        //public UserRolesData userRolesData { get; set; }
        public IList<UserRoles> userRoles { get; set; }

        public IList<IdentityUser> users { get; set; }
        public async Task OnGetAsync(bool NoRoles = true)
        {
            var test = NoRoles;

            userRoles = new List<UserRoles>();
            users = await _context.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            foreach (var u in users)
            {
                string role;
                var user = await _usermanager.FindByIdAsync(u.Id);
                var roles = await _usermanager.GetRolesAsync(user);
                if (NoRoles && roles.Count == 0)
                {
                    role = "������";
                    userRoles.Add(new UserRoles { RoleName = role, UserId = u.Id, UserName = u.UserName });
                }
                else if (!NoRoles && !(roles.Count == 0))
                {
                    role = roles[0];
                    userRoles.Add(new UserRoles { RoleName = role, UserId = u.Id, UserName = u.UserName });
                }
                else if (!NoRoles && roles.Count == 0)
                {
                    role = "������";
                    userRoles.Add(new UserRoles { RoleName = role, UserId = u.Id, UserName = u.UserName });
                }

                //if (roles.Count == 0) role = "������";
                //else role = roles[0];
                //userRoles.Add(new UserRoles { RoleName = role, UserId = u.Id, UserName = u.UserName });
            }

            var rolesQuery = _context.Roles.OrderBy(r=>r.Name);

            RoleNameSL = new SelectList(rolesQuery.AsNoTracking(),
                        "Name", "Name");
            //return Page();
        }

        public async Task<IActionResult> OnPostAsync(string mainid, string rolename)
        {
            //IEnumerable<string> roles = _context.Roles.Select(x => x.Name).OrderBy(x=>x).ToList();
            var user = await _usermanager.FindByIdAsync(mainid);
            var roles = await _usermanager.GetRolesAsync(user);
            await _usermanager.RemoveFromRolesAsync(user, roles);
            await _usermanager.AddToRoleAsync(user, rolename);
            return RedirectToPage("./Admin");
        }

    }
    public class UserRoles
    {
        public string UserId { get; set; }
        [Display(Name = "����������")]
        public string UserName { get; set; }
        [Display(Name = "������������")]
        public string RoleName { get; set; }
    }
}
