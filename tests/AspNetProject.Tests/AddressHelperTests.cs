using Xunit;
using AspNetProject.Utilities;

namespace AspNetProject.Tests;

public class AddressHelperTests
{
    [Fact]
    public void TestBasicStreet()
    {
        var input = "Naugarduko g. 121";
        
        var result = AddressHelper.GetStreetName(input);
        
        Assert.Equal("Naugarduko g.", result);
    }

    [Fact]
    public void TestAvenue()
    {
        var input = "Gedimino pr. 121";
        
        var result = AddressHelper.GetStreetName(input);
        
        Assert.Equal("Gedimino pr.", result);
    }

    [Fact]
    public void TestHighway()
    {
        var input = "Nemenčinės pl. 156";
        
        var result = AddressHelper.GetStreetName(input);
        
        Assert.Equal("Nemenčinės pl.", result);
    }

     [Fact]
    public void TestWithDistrict()
    {
        var input = "Kovo 11-osios g. 37, Grigiškės";
        
        var result = AddressHelper.GetStreetName(input);
        
        Assert.Equal("Kovo 11-osios g.", result);
    }
    
}