using System;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Liquid;
using OrchardCore.Workflows.Models;
using OrchardCore.Workflows.Services;

namespace OrchardCore.Workflows.Liquid
{
    public class SignalUrlFilter : ILiquidFilter
    {
        public Task<FluidValue> ProcessAsync(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            if (!context.AmbientValues.TryGetValue("UrlHelper", out var urlHelperObj))
            {
                throw new ArgumentException("UrlHelper missing while invoking 'signal_url'");
            }

            if (!context.AmbientValues.TryGetValue("SignalService", out var signalServiceObj))
            {
                throw new ArgumentException("SignalService missing while invoking 'signal_url'");
            }

            var workflowContextValue = context.GetValue(nameof(WorkflowExecutionContext));

            if (workflowContextValue.IsNil())
            {
                throw new ArgumentException("WorkflowExecutionContext missing while invoking 'signal_url'");
            }

            var workflowContext = (WorkflowExecutionContext)workflowContextValue.ToObjectValue();
            var signalName = input.ToStringValue();
            var payload = string.IsNullOrWhiteSpace(workflowContext.CorrelationId)
                ? SignalPayload.ForWorkflowInstance(signalName, workflowContext.WorkflowInstanceId)
                : SignalPayload.ForCorrelation(signalName, workflowContext.CorrelationId);

            var urlHelper = (IUrlHelper)urlHelperObj;
            var signalService = (ISecurityTokenService)signalServiceObj;
            var token = signalService.CreateToken(payload);
            var urlValue = new StringValue(urlHelper.Action("Trigger", "Signal", new { area = "OrchardCore.Workflows", token }));
            return Task.FromResult<FluidValue>(urlValue);
        }
    }
}
