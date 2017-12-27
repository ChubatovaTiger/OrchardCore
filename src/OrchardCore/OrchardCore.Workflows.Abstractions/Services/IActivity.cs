using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using OrchardCore.Entities;
using OrchardCore.Workflows.Abstractions.Models;
using OrchardCore.Workflows.Models;

namespace OrchardCore.Workflows.Services
{
    public interface IActivity : IEntity
    {
        string Name { get; }
        LocalizedString Category { get; }
        LocalizedString Description { get; }

        /// <summary>
        /// List of possible outcomes when the activity is executed.
        /// </summary>
        IEnumerable<Outcome> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext);

        /// <summary>
        /// Whether the activity can transition to the next outcome. Can prevent the activity from being transitioned
        /// because a condition is not valid.
        /// </summary>
        bool CanExecute(WorkflowContext workflowContext, ActivityContext activityContext);

        /// <summary>
        /// Executes the current activity.
        /// </summary>
        /// <returns>The names of the resulting outcomes.</returns>
        Task<IEnumerable<string>> ExecuteAsync(WorkflowContext workflowContext, ActivityContext activityContext);

        /// <summary>
        /// Called on each activity when a workflow is about to start.
        /// </summary>
        void OnWorkflowStarting(WorkflowContext context, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Called on each activity when a workflow has started.
        /// </summary>
        void OnWorkflowStarted(WorkflowContext context);

        /// <summary>
        /// Called on each activity when a workflow is about to be resumed.
        /// </summary>
        void OnWorkflowResuming(WorkflowContext context, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Called on each activity when a workflow is resumed.
        /// </summary>
        void OnWorkflowResumed(WorkflowContext context);

        /// <summary>
        /// Called on each activity when an activity is about to be executed.
        /// </summary>
        void OnActivityExecuting(WorkflowContext workflowContext, ActivityContext activityContext, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Called on each activity when an activity has been executed.
        /// </summary>
        void OnActivityExecuted(WorkflowContext workflowContext, ActivityContext activityContext);
    }
}