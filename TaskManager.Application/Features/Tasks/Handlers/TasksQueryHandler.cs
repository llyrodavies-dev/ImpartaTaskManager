using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.Common.Models;
using TaskManager.Application.Common.Statics;
using TaskManager.Application.Features.Tasks.DTOs;
using TaskManager.Application.Features.Tasks.Query;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Projections;
using Utility.Filtering;
using Utility.Filtering.Models;
using Utility.Mediator;

namespace TaskManager.Application.Features.Tasks.Handlers
{
    public class TasksQueryHandler : IRequestHandler<TasksQuery, PagedResponse<TaskItemDto>>
    {
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly FilterModelConfiguration<TaskItemProjection> _filterModelConfig;

        public TasksQueryHandler(FilterModelConfiguration<TaskItemProjection> filterModelConfig, IUserAuthorizationService userAuthorizationService, ITaskItemRepository taskItemRepository)
        {
            _filterModelConfig = filterModelConfig;
            _userAuthorizationService = userAuthorizationService;
            _taskItemRepository = taskItemRepository;
        }

        public async Task<PagedResponse<TaskItemDto>> Handle(TasksQuery request, CancellationToken cancellationToken = default)
        {
            User domainUser = await _userAuthorizationService.GetAuthenticatedUserAsync(cancellationToken);

            // Add the userId filter to restrict results to the authenticated user
            var filters = AddDefaultFilter(request.Filter, domainUser.Id);

            QueryFilterHelper.ValidateFilters(_filterModelConfig, filters);

            string newSort = QueryFilterHelper.GenerateSortExpression(_filterModelConfig, request.SortColumn, request.IsDescending, nameof(TaskItemProjection.Id));

            (string filterQuery, Dictionary<string, object> filterArgs) = QueryFilterHelper.ParseFilters(filters);

            PagedResponse<TaskItemProjection> pagedResponseProjection = await GetAllTaskItemsFilteredAsync(filterQuery, filterArgs, request.PageNumber, request.PageSize, newSort, cancellationToken);

            return new PagedResponse<TaskItemDto>
            {
                TotalCount = pagedResponseProjection.TotalCount,
                Items = MapProjectionToTaskItemDto(pagedResponseProjection.Items)
            };
        }

        private static List<FilterCondition> AddDefaultFilter(List<FilterCondition> filterConditions, Guid userId)
        {
            filterConditions.Add(new FilterCondition
            {
                FieldName = nameof(TaskItemProjection.UserId),
                Operator = FilterOperator.Equals,
                Values = [userId.ToString()],
                Type = typeof(Guid)
            });
            return filterConditions;
        }

        private async Task<PagedResponse<TaskItemProjection>> GetAllTaskItemsFilteredAsync(string filterQuery, Dictionary<string, object> filterArgs, int skip, int take, string sortColumn, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(sortColumn))
            {
                sortColumn = $"{nameof(TaskItemProjection.Id)}";
            }
            Func<IQueryable<TaskItemProjection>, IQueryable<TaskItemProjection>> orderBy = GenerateSort(sortColumn);
            return await _taskItemRepository.GetTaskItemsFilteredAsync(filterQuery, filterArgs, orderBy, cancellationToken, skip, take);
        }

        private static Func<IQueryable<TaskItemProjection>, IQueryable<TaskItemProjection>> GenerateSort(string sortColumn)
        {
            if (sortColumn.Contains(nameof(TaskItemProjection.Status)))
            {
                Expression<Func<TaskItemProjection, string>> order = GetExpressionForTaskStatus();
                return query => sortColumn.Contains(" DESC")
                    ? query.OrderByDescending(order).ThenBy(x => x.Id)
                    : query.OrderBy(order).ThenBy(x => x.Id);
            }

            return query => query.OrderBy(sortColumn).ThenBy(x => x.Id);
        }

        private static List<TaskItemDto> MapProjectionToTaskItemDto(List<TaskItemProjection> taskItemProjections)
            => [.. taskItemProjections.Select(x => new TaskItemDto
            {
                Id = x.Id,
                JobId = x.JobId,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status,
                CreatedAtUtc = x.CreatedAtUtc,
                CreatedBy = x.CreatedBy,
                ModifiedAtUtc = x.ModifiedAtUtc,
                ModifiedBy = x.ModifiedBy
            })];

#pragma warning disable S3358 // Ternary operators should not be nested
        // To ensure compatibility with Entity Framework, we need to keep the logic simple and inline within the expression.
        // Thus we are using a series of if-else conditions within the expression to ensure it translates correctly to Entity Framework.
        public static Expression<Func<TaskItemProjection, string>> GetExpressionForTaskStatus()
        {
            return y => (int)y.Status == (int)TaskItemStatus.Unspecified ? TaskItemStatus.Unspecified.ToString()
                        : (int)y.Status == (int)TaskItemStatus.NotStarted ? TaskItemStatus.NotStarted.ToString()
                        : (int)y.Status == (int)TaskItemStatus.InProgress ? TaskItemStatus.InProgress.ToString()
                        : (int)y.Status == (int)TaskItemStatus.Completed ? TaskItemStatus.Completed.ToString()
                        : (int)y.Status == (int)TaskItemStatus.Blocked ? TaskItemStatus.Blocked.ToString()
                        : (int)y.Status == (int)TaskItemStatus.Cancelled ? TaskItemStatus.Cancelled.ToString()
                        : TaskItemStatus.Unspecified.ToString();
        }
#pragma warning restore S3358 // Ternary operators should not be nested
    }
}
