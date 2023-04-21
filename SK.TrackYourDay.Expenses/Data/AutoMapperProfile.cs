using AutoMapper;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.Expenses.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ExpenseDTO, ExpenseVM>();
            CreateMap<ExpenseVM, ExpenseDTO>();
        }
    }
}
