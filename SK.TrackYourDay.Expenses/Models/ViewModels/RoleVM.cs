using Microsoft.AspNetCore.Mvc.Rendering;

namespace SK.TrackYourDay.Expenses.Models.ViewModels
{
    public static class RoleVM
    {
        public static string Admin = "Admin";
        public static string User = "User";
        public static string TeamMate = "TeamMate";

        public static List<SelectListItem> GetRolesForDropDown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = RoleVM.Admin, Text = RoleVM.Admin },
                new SelectListItem { Value = RoleVM.User, Text = RoleVM.User },
                new SelectListItem { Value = RoleVM.TeamMate, Text = RoleVM.TeamMate }
            };
        }
    }
}
