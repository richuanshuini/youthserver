using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace YouthApartmentServer.Common.Validation;

public static class ModelStateHelper
{
    public static List<string> CollectModelErrors(ModelStateDictionary modelState)
    {
        return modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "参数验证失败" : e.ErrorMessage)
            .ToList();
    }
}
