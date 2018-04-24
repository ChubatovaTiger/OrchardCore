using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json.Linq;
using OrchardCore.Workflows.Models;

namespace OrchardCore.Workflows.Services
{
    public interface IWorkflowManager
    {
        /// <summary>
        /// Creates a new workflow instance for the specified workflow definition.
        /// </summary>
        WorkflowInstance NewWorkflowInstance(WorkflowDefinition workflowDefinitionRecord, string correlationId = null);

        /// <summary>
        /// Creates a new <see cref="WorkflowExecutionContext"/>.
        /// </summary>
        Task<WorkflowExecutionContext> CreateWorkflowExecutionContextAsync(WorkflowDefinition workflowDefinition, WorkflowInstance workflowInstanceRecord, IDictionary<string, object> input = null);

        /// <summary>
        /// Creates a new <see cref="ActivityContext"/>.
        /// </summary>
        /// <param name="activityRecord"></param>
        Task<ActivityContext> CreateActivityExecutionContextAsync(ActivityRecord activityRecord, JObject properties);

        /// <summary>
        /// Triggers a specific <see cref="IEvent"/>.
        /// </summary>
        /// <param name="name">The type of the event to trigger, e.g. ContentPublishedEvent.</param>
        /// <param name="input">An object containing context for the event.</param>
        /// <param name="correlationId">Optionally specify a application-specific value to associate the workflow instance with. For example, a content item ID.</param>
        Task TriggerEventAsync(string name, IDictionary<string, object> input = null, string correlationId = null);

        /// <summary>
        /// Starts a new workflow using the specified workflow definition.
        /// </summary>
        /// <param name="workflowDefinition">The workflow definition to start.</param>
        /// <param name="input">Optionally specify any inputs to be used by the workflow.</param>
        /// <param name="correlationId">Optionally specify a application-specific value to associate the workflow instance with. For example, a content item ID.</param>
        /// <param name="startActivityName">If a workflow definition contains multiple start activities, you can specify which one to use. If none specified, the first one will be used.</param>
        /// <returns>Returns the created workflow context. Can be used for further inspection of the workflow state.</returns>
        Task<WorkflowExecutionContext> StartWorkflowAsync(WorkflowDefinition workflowDefinition, ActivityRecord startActivity = null, IDictionary<string, object> input = null, string correlationId = null);

        /// <summary>
        /// Resumes the specified workflow instance at the specified activity.
        /// </summary>
        Task<WorkflowExecutionContext> ResumeWorkflowAsync(WorkflowInstance workflowInstance, BlockingActivity awaitingActivity, IDictionary<string, object> input = null);

        /// <summary>
        /// Executes the specified workflow starting at the specified activity.
        /// </summary>
        Task<IEnumerable<ActivityRecord>> ExecuteWorkflowAsync(WorkflowExecutionContext workflowExecutionContext, ActivityRecord activity);
    }

    public static class WorkflowManagerExtensions
    {
        public static Task TriggerEventAsync(this IWorkflowManager workflowManager, string name, object input = null, string correlationId = null)
        {
            return workflowManager.TriggerEventAsync(name, new RouteValueDictionary(input), correlationId);
        }
    }
}