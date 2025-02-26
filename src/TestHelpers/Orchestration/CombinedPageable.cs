using Microsoft.DurableTask;

namespace SFA.DAS.Funding.ApprenticeshipPayments.TestHelpers.Orchestration;

public class CombinedPageable<T> : AsyncPageable<T>
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
    public static IEnumerable<T> ToList<T>(this AsyncPageable<T> pageable)
    {
        var page = pageable.AsPages().GetAsyncEnumerator().Current; // There will only be one page with this fake implementation
        return page.Values;
    }
}

public class FakeAsyncEnumerable<T> : IAsyncEnumerable<Page<T>>
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

public class FakeAsyncEnumerator<T> : IAsyncEnumerator<Page<T>>
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