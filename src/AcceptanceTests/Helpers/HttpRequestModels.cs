using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Security.Claims;

namespace SFA.DAS.Funding.ApprenticeshipPayments.AcceptanceTests.Helpers;

public class TestFunctionContext : FunctionContext
{
    // You can mock required properties/methods here as needed
    // or forward calls to a real context
    public override string InvocationId => throw new NotImplementedException();

    public override string FunctionId => throw new NotImplementedException();

    public override TraceContext TraceContext => throw new NotImplementedException();

    public override BindingContext BindingContext => throw new NotImplementedException();

    public override RetryContext RetryContext => throw new NotImplementedException();

    public override IServiceProvider InstanceServices { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override FunctionDefinition FunctionDefinition => throw new NotImplementedException();

    public override IDictionary<object, object> Items { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override IInvocationFeatures Features => throw new NotImplementedException();
}

public class TestHttpRequestData : HttpRequestData
{
    private FunctionContext _functionContext;

    public static TestHttpRequestData New()
    {
        return new TestHttpRequestData(new TestFunctionContext());
    }

    public TestHttpRequestData(FunctionContext context) : base(context)
    {
        _functionContext = context;
    }

    public override Stream Body => throw new NotImplementedException();

    public override HttpHeadersCollection Headers => throw new NotImplementedException();

    public override IReadOnlyCollection<IHttpCookie> Cookies => throw new NotImplementedException();

    public override Uri Url => throw new NotImplementedException();

    public override IEnumerable<ClaimsIdentity> Identities => throw new NotImplementedException();

    public override string Method => throw new NotImplementedException();

    public override HttpResponseData CreateResponse()
    {
        return new TestHttpResponseData(_functionContext);
    }
}

public class TestHttpResponseData : HttpResponseData
{
    public TestHttpResponseData(FunctionContext context) : base(context)
    {
        Headers = new HttpHeadersCollection();
        Body = new MemoryStream();
    }

    public override HttpStatusCode StatusCode { get; set; }
    public override HttpHeadersCollection Headers { get; set; }
    public override Stream Body { get; set; }

    public override HttpCookies Cookies => throw new NotImplementedException();
}
