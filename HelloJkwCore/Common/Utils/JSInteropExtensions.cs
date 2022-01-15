using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common;

public static class JSInteropExtensions
{
    public static ValueTask FocusAsync(this IJSRuntime jsRuntime, ElementReference elementReference)
    {
        return jsRuntime.InvokeVoidAsync("BlazorFocusElement", elementReference);
    }
    public static ValueTask ConsoleLogAsync(this IJSRuntime jsRuntime, string logLevel, params object[] args)
    {
        var newArgs = new object[] { logLevel }.Concat(args).ToArray();
        return jsRuntime.InvokeVoidAsync("ConsoleLog", newArgs);
    }
}