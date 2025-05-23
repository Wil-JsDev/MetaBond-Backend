﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressEntries;

    internal sealed class GetProgressBoardIdWithEntriesQueryHandler(
        IProgressBoardRepository repository,
        IDistributedCache decoratedCache,
        IProgressEntryRepository progressEntryRepository,
        ILogger<GetProgressBoardIdWithEntriesQueryHandler> logger)
        : IQueryHandler<GetProgressBoardIdWithEntriesQuery, IEnumerable<ProgressBoardWithProgressEntryDTos>>
    {
        public async Task<ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>> Handle(
            GetProgressBoardIdWithEntriesQuery request, 
            CancellationToken cancellationToken)
        {

            var progressBoard = await repository.GetByIdAsync(request.ProgressBoardId);
            if (progressBoard != null)
            {
                string cacheKey = $"get-board-id-with-entries-{request.ProgressBoardId}";
                var progressBoardList = await decoratedCache.GetOrCreateAsync(
                    cacheKey,
                    async () => await repository.GetBoardsWithEntriesAsync(request.ProgressBoardId, cancellationToken), 
                    cancellationToken: cancellationToken);
                
                var progressBoards = progressBoardList.ToList();
                if (!progressBoards.Any())
                {
                    logger.LogError("No progress entries found for ProgressBoardId: {ProgressBoardId}", request.ProgressBoardId);

                    return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                if (request.PageNumber <= 0 || request.PageSize <= 0)
                {
                    logger.LogWarning("Invalid pagination parameters: Page = {Page}, PageSize = {PageSize}", request.PageNumber, request.PageSize);

                    return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
                        Error.Failure("400", "Page number and page size must be greater than zero. Please provide valid pagination values."));
                }

                var progressEntryList = await decoratedCache.GetOrCreateAsync(
                    $"progress-entry-paged-with-board-{request.PageNumber}-{request.PageSize}",
                    async () => await progressEntryRepository.GetPagedProgressEntryAsync(
                        request.PageNumber,
                        request.PageSize,
                        cancellationToken), 
                    cancellationToken: cancellationToken);
                
               var progressEntry = progressEntryList.Items!.Select(x => 
                    new ProgressEntrySummaryDTos
                (
                    ProgressEntryId: x.Id,
                    Description: x.Description,
                    UserId: x.UserId,
                    CreatedAt: x.CreatedAt,
                    ModifiedAt: x.UpdateAt
                )).ToList();
                
                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDs = progressBoards.Select(x => new ProgressBoardWithProgressEntryDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    UserId: x.UserId,
                    ProgressEntries:progressEntry,
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt
                ));

                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDTosEnumerable =
                    progressBoardWithProgressEntryDs.ToList();
                logger.LogInformation("Successfully retrieved {Count} progress entries for ProgressBoardId: {ProgressBoardId}",
                 progressBoardWithProgressEntryDTosEnumerable.Count(), request.ProgressBoardId);

                return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(progressBoardWithProgressEntryDTosEnumerable);
            }
            logger.LogError("ProgressBoard with ID {ProgressBoardId} not found.", request.ProgressBoardId);

            return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.NotFound("404", "Progress board not found"));
        }
    }

