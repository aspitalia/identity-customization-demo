using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IdentityCustomizationDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IdentityCustomizationDemo.Areas.Identity.Pages.Account.Manage
{
    public class DownloadPersonalDataModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DownloadPersonalDataModel> _logger;

        public DownloadPersonalDataModel(
            UserManager<ApplicationUser> userManager,
            ILogger<DownloadPersonalDataModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, object>();
            var queue = new Queue<(Dictionary<string, object> Data, object Source)>();
            queue.Enqueue((personalData, user));
            while (queue.Any())
            {
                var obj = queue.Dequeue();
                var properties = obj.Source.GetType().GetProperties()
                    .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)))
                    .ToList();
                foreach (var prop in properties) 
                {
                    var value = prop.GetValue(obj.Source);
                    if (IsSimpleValue(value))
                    {
                        obj.Data.Add(prop.Name, value?.ToString() ?? null);
                    }
                    else if (IsEnumerable(value))
                    {
                        var list = new List<Dictionary<string, object>>();
                        var enumerator = ((IEnumerable) value).GetEnumerator();
                        while(enumerator.MoveNext())
                        {
                            var dictionary = new Dictionary<string, object>();
                            list.Add(dictionary);
                            queue.Enqueue((dictionary, enumerator.Current));
                        }
                        obj.Data.Add(prop.Name, list);
                    }
                    else
                    {
                        var dictionary = new Dictionary<string, object>();
                        queue.Enqueue((dictionary, value));
                        obj.Data.Add(prop.Name, dictionary);
                    }
                }
            }

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)), "text/json");
        }

        private bool IsSimpleValue(object value)
        {
            if (value == null) {
                return true;
            } else if (value.GetType().IsPrimitive) {
                return true;
            } else if (value.GetType() == typeof(string) || value.GetType() == typeof(decimal)) {
                return true;
            }
            return false;
        }

        private bool IsEnumerable(object value)
        {
            if (value == null)
            {
                return false;
            }
            return typeof(IEnumerable).IsAssignableFrom(value.GetType());
        }
    }
}
