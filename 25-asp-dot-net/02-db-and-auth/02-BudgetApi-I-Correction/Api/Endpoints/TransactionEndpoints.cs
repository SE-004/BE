using BudgetApi.Dtos.Transactions;
using BudgetApi.Application.Interfaces;


namespace BudgetApi.Api.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/transactions").WithTags("Transactions");

        // GET /transactions
        group.MapGet("/", async (ITransactionService transactionService) =>
        {
            var transactions = await transactionService.ListAsync();
            var transactionDtos = transactions.Select(p => new TransactionResponseDto(p.Id, p.Timestamp, p.Type, p.Description, p.Amount, p.Date));
            return TypedResults.Ok(transactionDtos);
        })
        .Produces<IEnumerable<TransactionResponseDto>>();

        // GET /transactions/{id:guid}
        group.MapGet("/{id:guid}", async (Guid id, ITransactionService transactionService) =>
        {
            var transaction = await transactionService.GetAsync(id);
            if (transaction is null)
                return Results.Problem(detail: "Transaction not found", statusCode: StatusCodes.Status404NotFound);

            var transactionDto = new TransactionResponseDto(transaction.Id, transaction.Timestamp, transaction.Type, transaction.Description, transaction.Amount, transaction.Date);
            return TypedResults.Ok(transactionDto);
        })
        .Produces<TransactionResponseDto>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        // POST /transactions
        group.MapPost("/", async (CreateTransactionDto createTransactionDto, ITransactionService transactionService, HttpContext context) =>
        {
            var transaction = await transactionService.CreateAsync(createTransactionDto);
            var transactionDto = new TransactionResponseDto(transaction.Id, transaction.Timestamp, transaction.Type, transaction.Description, transaction.Amount, transaction.Date);

            var location = $"{context.Request.Scheme}://{context.Request.Host}/transactions/{transaction.Id}";
            return TypedResults.Created(location, transactionDto);

        })
        .Produces<TransactionResponseDto>(StatusCodes.Status201Created);

        // PATCH /transactions/{id:guid}
        group.MapPatch("/{id:guid}", async (Guid id, UpdateTransactionDto updateTransactionDto, ITransactionService transactionService) =>
        {
            var transaction = await transactionService.UpdateAsync(id, updateTransactionDto);
            if (transaction is null)
                return Results.Problem(detail: "Transaction not found", statusCode: StatusCodes.Status404NotFound);

            var transactionDto = new TransactionResponseDto(transaction.Id, transaction.Timestamp, transaction.Type, transaction.Description, transaction.Amount, transaction.Date);
            return TypedResults.Ok(transactionDto);
        })
        .Produces<TransactionResponseDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        // DELETE /transactions/{id:guid}
        group.MapDelete("/{id:guid}", async (Guid id, ITransactionService transactionService) =>
        {
            var deleted = await transactionService.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.Problem(detail: "Transaction not found", statusCode: StatusCodes.Status404NotFound);
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}