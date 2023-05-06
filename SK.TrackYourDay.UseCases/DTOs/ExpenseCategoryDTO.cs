using SK.TrackYourDay.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.TrackYourDay.UseCases.DTOs
{
    public class ExpenseCategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string User { get; set; }
    }
}
