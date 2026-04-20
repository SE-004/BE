using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


using BudgetApi.Dtos.Transactions;
using BudgetApi.Application.Interfaces;
using BudgetApi.Api.Filters;

namespace BudgetApi.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions").WithTags("Transactions");

        // GET /transactions
        group.MapGet("/", [Authorize] async (ITransactionService transactionService, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var transactions = await transactionService.ListAsync();
            var transactionDtos = transactions.Where(p => p.UserId == userId)
                                            .Select(p => new TransactionResponseDto(p.Id, p.UserId, p.Timestamp, p.Type, p.Description, p.Amount, p.Date));
            return TypedResults.Ok(transactionDtos);
        })
        .Produces<IEnumerable<TransactionResponseDto>>();

        // GET /transactions/{id:guid}
        group.MapGet("/{id:guid}", [Authorize] async (Guid id, ITransactionService transactionService, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var transaction = await transactionService.GetAsync(id);
            if (transaction is null)
                return Results.Problem(detail: "Transaction not found", statusCode: StatusCodes.Status404NotFound);

            if (transaction.UserId != userId)
                return Results.Problem(detail: "Forbidden", statusCode: StatusCodes.Status403Forbidden);

            var transactionDto = new TransactionResponseDto(transaction.Id, transaction.UserId, transaction.Timestamp, transaction.Type, transaction.Description, transaction.Amount, transaction.Date);
            return TypedResults.Ok(transactionDto);
        })
        .Produces<TransactionResponseDto>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        // POST /transactions
        group.MapPost("/", [Authorize] async (CreateTransactionDto createTransactionDto, ITransactionService transactionService, HttpContext context, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var transaction = await transactionService.CreateAsync(userId, createTransactionDto);
            var transactionDto = new TransactionResponseDto(transaction.Id, transaction.UserId, transaction.Timestamp, transaction.Type, transaction.Description, transaction.Amount, transaction.Date);

            var location = $"{context.Request.Scheme}://{context.Request.Host}/transactions/{transaction.Id}";
            return TypedResults.Created(location, transactionDto);

        })
        .Produces<TransactionResponseDto>(StatusCodes.Status201Created)
        .WithValidation<CreateTransactionDto>();

        // PATCH /transactions/{id:guid}
        group.MapPatch("/{id:guid}", [Authorize] async (Guid id, UpdateTransactionDto updateTransactionDto, ITransactionService transactionService, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var initialTransaction = await transactionService.GetAsync(id);
            if (initialTransaction is null)
                return Results.Problem(detail: "Transaction not found", statusCode: StatusCodes.Status404NotFound);

            if (initialTransaction.UserId != userId)
                return Results.Problem(detail: "Forbidden", statusCode: StatusCodes.Status403Forbidden);

            var transaction = await transactionService.UpdateAsync(id, updateTransactionDto);
            if (transaction is null)
                return Results.Problem(detail: "Transaction not found", statusCode: StatusCodes.Status404NotFound);

            var transactionDto = new TransactionResponseDto(transaction.Id, transaction.UserId, transaction.Timestamp, transaction.Type, transaction.Description, transaction.Amount, transaction.Date);
            return TypedResults.Ok(transactionDto);
        })
        .WithValidation<UpdateTransactionDto>()
        .Produces<TransactionResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // DELETE /transactions/{id:guid}
        group.MapDelete("/{id:guid}", [Authorize] async (Guid id, ITransactionService transactionService, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var initialTransaction = await transactionService.GetAsync(id);
            if (initialTransaction is null)
                return Results.Problem(detail: "Transaction not found", statusCode: StatusCodes.Status404NotFound);

            if (initialTransaction.UserId != userId)
                return Results.Problem(detail: "Forbidden", statusCode: StatusCodes.Status403Forbidden);

            var deleted = await transactionService.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.Problem(detail: "Transaction not found", statusCode: StatusCodes.Status404NotFound);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}