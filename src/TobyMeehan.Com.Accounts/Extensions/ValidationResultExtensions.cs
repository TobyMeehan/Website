using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TobyMeehan.Com.Accounts.Extensions;

public static class ValidationResultExtensions
{
    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
    {
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }

    public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState, string modelName)
    {
        foreach (var error in result.Errors)
        {
            modelState.AddModelError($"{modelName}.{error.PropertyName}", error.ErrorMessage);
        }
    }
}