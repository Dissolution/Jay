namespace Jay.Tests.ResultTests;

public class ImplicitTests
{
    [Fact]
    public void CanCastFromBool()
    {
        Result trueResult = (bool)true;
        Assert.Equal(Result.Ok(), trueResult);
        
        Result falseResult = (bool)false;
        Assert.NotEqual(Result.Ok(), falseResult);
    }
    
    [Fact]
    public void CanCastFromException()
    {
        Result nullErrorResult = (Exception?)(null);
        Assert.NotEqual(Result.Ok(), nullErrorResult);

        Result errorResult = (Exception?)(new Exception());
        Assert.NotEqual(Result.Ok(), errorResult);
    }
    
    [Fact]
    public void CanCastToBool()
    {
        bool t = Result.Ok();
        Assert.True(t);

        bool f = Result.Error(null);
        Assert.False(f);
    }
}