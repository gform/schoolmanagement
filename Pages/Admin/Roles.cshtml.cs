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
    public class RolesModel : PageModel
    {
        public SelectList RoleNameSL { get; set; }
        private readonly Data.ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _usermanager;
        //private readonly RoleManager<IdentityUser> _rolemanager;
        private readonly IConfiguration Configuration;
        public PaginatedList<UserRoles> userRolesPaginated { get; set; }
        public bool NoRoles { get; set; }

        public RolesModel(sms.Data.ApplicationDbContext context, 
            UserManager<IdentityUser> usermanager, 
            IConfiguration configuration)
        {
            _context = context;
            _usermanager = usermanager;
            Configuration = configuration;
            //NoRoles = true;
        }
        //public UserRolesData userRolesData { get; set; }
        public IList<UserRoles> userRoles { get; set; }

        public IList<IdentityUser> users { get; set; }
        public async Task OnGetAsync(bool noRoles, int? pageIndex)
        {
            NoRoles = noRoles;
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
                    role = "��� �����������";
                    userRoles.Add(new UserRoles { RoleName = role, UserId = u.Id, UserName = u.UserName });
                }
                else if (!NoRoles && !(roles.Count == 0))
                {
                    role = roles[0];
                    userRoles.Add(new UserRoles { RoleName = role, UserId = u.Id, UserName = u.UserName });
                }
                else if (!NoRoles && roles.Count == 0)
                {
                    role = "��� �����������";
                    userRoles.Add(new UserRoles { RoleName = role, UserId = u.Id, UserName = u.UserName });
                }
            }

            //Dropdown for roles
            var rolesQuery = _context.Roles.OrderBy(r => r.Name);
            RoleNameSL = new SelectList(rolesQuery.AsNoTracking(),
                        "Name", "Name");
            
            //Pagination
            var pageSize = Configuration.GetValue("PageSize", 5);
            userRolesPaginated = PaginatedList<UserRoles>.CreateFromList(
                userRoles, pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnPostAsync(string mainid, string rolename, bool noRoles, int? pageIndex)
        {
            //IEnumerable<string> roles = _context.Roles.Select(x => x.Name).OrderBy(x=>x).ToList();
            var rolesQuery = _context.Roles.OrderBy(r => r.Name).ToList();
            string result = string.Join(",", rolesQuery);
            var user = await _usermanager.FindByIdAsync(mainid);
            var roles = await _usermanager.GetRolesAsync(user);
            await _usermanager.RemoveFromRolesAsync(user, roles);
            //noroles is not added (or you get error)
            if (result.Contains(rolename)) await _usermanager.AddToRoleAsync(user, rolename);
            //return RedirectToAction("OnGetAsync", new { noRoles = "noRoles", pageIndex = "pageIndex" });
            return RedirectToPage("./Admin/Roles", new { noRoles = noRoles, pageIndex = pageIndex });
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