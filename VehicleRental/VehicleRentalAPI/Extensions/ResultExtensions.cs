using VehicleRental.Domain.Entities.Base;

namespace VehicleRentalAPI.Extensions;

public static class ResultExtensions
{
    public static async Task<Result<TDestination>> MapResultTo<TSource, TDestination>(this Task<Result<TSource>> task, Func<TSource, TDestination> mapper)
    {
        Result<TSource> result = await task;
        return result.MapResultTo(mapper);
    }

    public static Result<TDestination> MapResultTo<TSource, TDestination>(this Result<TSource> result, Func<TSource, TDestination> mapper)
    {
        return result.IsSuccess
               ? Result<TDestination>.Success(mapper(result.Data!))
               : Result<TDestination>.Failure(result.ErrorMessage!, result.ErrorType!.Value);
    }

    public static async Task<Result<List<TDestination>>> MapResultTo<TList, TSource, TDestination>(this Task<Result<TList>> task,
                                                                                                   Func<TSource, TDestination> mapper) where TList : IEnumerable<TSource>
    {
        Result<TList> result = await task;
        return result.MapResultTo(mapper);
    }

    public static Result<List<TDestination>> MapResultTo<TList, TSource, TDestination>(this Result<TList> result,
                                                                                       Func<TSource, TDestination> mapper) where TList : IEnumerable<TSource>
    {
        if (result.IsSuccess)
        {
            List<TDestination> mappedList = [.. result.Data!.Select(x => mapper(x))];
            return Result<List<TDestination>>.Success(mappedList);
        }
        return Result<List<TDestination>>.Failure(result.ErrorMessage!, result.ErrorType!.Value);
    }
}
