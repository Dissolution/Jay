namespace Jay.Reflection.Tests.Adapters;

public class TestEvents
{
    [Fact]
    public void TestAddHandler()
    {
        int counter = 0;
        var testNotify = new TestNotify();
        
        testNotify.PropertyChanged += notifyOnPropertyChanged;
        Assert.Equal(0, counter);
        testNotify.Id = EntityGenerator.New<int>();
        Assert.Equal(1, counter);
        
        testNotify.PropertyChanged -= notifyOnPropertyChanged;
        Assert.Equal(1, counter);
        testNotify.Id = EntityGenerator.New<int>();
        Assert.Equal(1, counter);

        var propertyChangedEvent = typeof(TestNotify)
            .GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(e => e.Name == "PropertyChanged")
            .OneOrDefault();
        
        //var propertyChangedEvent = MemberSearch.One<TestNotify, EventInfo>("PropertyChanged");
        
        
        propertyChangedEvent!.AddHandler(ref testNotify, notifyOnPropertyChanged);
        Assert.Equal(1, counter);
        testNotify.Id = EntityGenerator.New<int>();
        Assert.Equal(2, counter);
        return;


        void notifyOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Interlocked.Increment(ref counter);
            // Console.WriteLine($"{sender} : {e.PropertyName}");
        }
    }

}