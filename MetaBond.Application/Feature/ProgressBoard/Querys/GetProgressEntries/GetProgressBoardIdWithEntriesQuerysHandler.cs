﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetProgressEntries;

    internal sealed class GetProgressBoardIdWithEntriesQuerysHandler(
        IProgressBoardRepository repository,
        IProgressEntryRepository progressEntryRepository,
        ILogger<GetProgressBoardIdWithEntriesQuerysHandler> logger)
        : IQueryHandler<GetProgressBoardIdWithEntriesQuerys, IEnumerable<ProgressBoardWithProgressEntryDTos>>
    {
        public async Task<ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>> Handle(
            GetProgressBoardIdWithEntriesQuerys request, 
            CancellationToken cancellationToken)
        {

            var progressBoard = await repository.GetByIdAsync(request.ProgressBoardId);
            if (progressBoard != null)
            {
                var progressBoardList = await repository.GetBoardsWithEntriesAsync(progressBoard.Id,cancellationToken);
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
                
                var progressEntryList = await progressEntryRepository.GetPagedProgressEntryAsync(
                    request.PageNumber,
                    request.PageSize, 
                    cancellationToken);
                
               var progressEntry = progressEntryList.Items!.Select(x => 
                    new ProgressEntrySummaryDTos
                (
                    ProgressEntryId: x.Id,
                    Description: x.Description,
                    CreatedAt: x.CreatedAt,
                    ModifiedAt: x.UpdateAt
                ));
                
                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDs = progressBoards.Select(x => new ProgressBoardWithProgressEntryDTos
                (
                    ProgressBoardId: x.Id,
                    CommunitiesId: x.CommunitiesId,
                    ProgressEntries: x.ProgressEntries != null ? 
                    progressEntry.Select(summaryDTos => new ProgressEntrySummaryDTos
                    (
                        ProgressEntryId: summaryDTos.ProgressEntryId,
                        Description: summaryDTos.Description,
                        CreatedAt: summaryDTos.CreatedAt,
                        ModifiedAt: summaryDTos.ModifiedAt
                    )).ToList() : [],
                    CreatedAt: x.CreatedAt,
                    UpdatedAt: x.UpdatedAt
                ));

                IEnumerable<ProgressBoardWithProgressEntryDTos> progressBoardWithProgressEntryDTosEnumerable = progressBoardWithProgressEntryDs as ProgressBoardWithProgressEntryDTos[] ?? progressBoardWithProgressEntryDs.ToArray();
                logger.LogInformation("Successfully retrieved {Count} progress entries for ProgressBoardId: {ProgressBoardId}",
                 progressBoardWithProgressEntryDTosEnumerable.Count(), request.ProgressBoardId);

                return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(progressBoardWithProgressEntryDTosEnumerable);
            }
            logger.LogError("ProgressBoard with ID {ProgressBoardId} not found.", request.ProgressBoardId);

            return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.NotFound("404", "Progress board not found"));
        }
    }

