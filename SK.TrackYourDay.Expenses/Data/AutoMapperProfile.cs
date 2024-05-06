using AutoMapper;
using SK.TrackYourDay.Expenses.Models.ViewModels;
using SK.TrackYourDay.UseCases.DTOs;

namespace SK.TrackYourDay.Expenses.Data;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<ExpenseDTO, ExpenseVM>();
        CreateMap<ExpenseVM, ExpenseDTO>();

        CreateMap<ExpenseCategoryDTO, ExpenseCategoryVM>();
        CreateMap<ExpenseCategoryVM, ExpenseCategoryDTO>();

        CreateMap<PaymentMethodDTO, PaymentMethodVM>();
        CreateMap<PaymentMethodVM, PaymentMethodDTO>();

        CreateMap<FilterDTO, FilterVM>();
        CreateMap<FilterVM, FilterDTO>();

        CreateMap<FilterDTO, FilterToCsvVM>();
        CreateMap<FilterToCsvVM, FilterDTO>();
    }
}
