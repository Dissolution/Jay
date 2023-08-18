namespace Jay.Tests.ResultTests;

public class ResultErrorTests
{
    [Fact]
    public void Result_Error_HasException()
    {
        Result error = Result.Error(new InvalidOperationException("Invalid Op"));
        Assert.True(error.IsError(out var ex));
        Assert.True(ex is not null);
        Assert.Equal(typeof(InvalidOperationException), ex.GetType());
        Assert.Equal("Invalid Op", ex.Message);
        
        Assert.False(error.IsOk());
    }
    
    [Fact]
    public void Result_Implicit_Exception_HasException()
    {
        Result error = new InvalidOperationException("Invalid Op");
        Assert.True(error.IsError(out var ex));
        Assert.True(ex is not null);
        Assert.Equal(typeof(InvalidOperationException), ex.GetType());
        Assert.Equal("Invalid Op", ex.Message);
        
        Assert.False(error.IsOk());
    }
    
    [Fact]
    public void Result_Implicit_False_HasException()
    {
        Result error = false;
        Assert.True(error.IsError(out var ex));
        Assert.True(ex is not null);
        Assert.Equal(typeof(Exception), ex.GetType());
        Assert.Equal("Operation Failed", ex.Message);
        
        Assert.False(error.IsOk());
    }
    
    [Fact]
    public void Result_New_HasException()
    {
        Result error = new();
        Assert.True(error.IsError(out var ex));
        Assert.True(ex is not null);
        Assert.Equal(typeof(Exception), ex.GetType());
        Assert.Equal("Operation Failed", ex.Message);
        
        Assert.False(error.IsOk());
    }
    
    [Fact]
    public void Result_Default_HasException()
    {
        Result error = default;
        Assert.True(error.IsError(out var ex));
        Assert.True(ex is not null);
        Assert.Equal(typeof(Exception), ex.GetType());
        Assert.Equal("Operation Failed", ex.Message);
        
        Assert.False(error.IsOk());
    }
}