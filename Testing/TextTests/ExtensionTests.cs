using Shouldly;

namespace TextTests;

public class ExtensionTests
{
    public const string Lorem = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus euismod eget nulla sit amet feugiat. Aenean laoreet, dolor eu gravida faucibus, metus erat condimentum dolor, ac vestibulum eros turpis sit amet massa. Nunc sit amet quam augue. Integer imperdiet quis ex at aliquam. Donec mollis vulputate arcu, ac consectetur neque rhoncus eget. Vestibulum quis malesuada urna. Nulla viverra quam sed dolor blandit molestie quis id lorem. Ut ultricies vulputate turpis, sit amet efficitur justo ultricies et.";


    [Fact]
    public void TestAsString()
    {
        ReadOnlySpan<char> readOnlySpan = Lorem;

        string? asString = new string(readOnlySpan);
        asString.ShouldBe(Lorem);

        string? toString = readOnlySpan.ToString();
        toString.ShouldBe(Lorem);
    }
}