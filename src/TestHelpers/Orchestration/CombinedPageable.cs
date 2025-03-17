using Microsoft.DurableTask;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

/// <summary>
/// This implementation is not intended to mimic the real behaviour of an AsyncPageable, 
/// it is just a simple implementation that will allow all results to be returned in one page.
/// </summary>
public class CombinedPageable<T> : AsyncPageable<T> where T : notnull
{
    private readonly IReadOnlyList<T> _items;

    public CombinedPageable(IEnumerable<T> items)
    {
        _items = items.ToList().AsReadOnly();
    }

    public override IAsyncEnumerable<Page<T>> AsPages(string? continuationToken = null, int? pageSizeHint = null)
    {
        return new FakeAsyncEnumerable<T>(_items);
    }
}

public static class AsyncPageableExtensions
{
    public static IEnumerable<T> ToList<T>(this AsyncPageable<T> pageable) where T : notnull
    {
        var page = pageable.AsPages().GetAsyncEnumerator().Current; // There will only be one page with this fake implementation
        return page.Values;
    }
}

public class FakeAsyncEnumerable<T> : IAsyncEnumerable<Page<T>> where T : notnull
{
    private readonly Page<T> _page;

    public FakeAsyncEnumerable(IReadOnlyList<T> page)
    {
        _page = new Page<T>(page);
    }

    public IAsyncEnumerator<Page<T>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new FakeAsyncEnumerator<T>(_page);
    }
}

public class FakeAsyncEnumerator<T> : IAsyncEnumerator<Page<T>> where T : notnull
{
    public Page<T> Current => _page;

    private readonly Page<T> _page;

    public FakeAsyncEnumerator(Page<T> page)
    {
        _page = page;
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new ValueTask<bool>(false);//Only one page
    }
}