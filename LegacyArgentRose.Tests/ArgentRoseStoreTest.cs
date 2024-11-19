using NUnit.Framework;
using static ArgentRose.Tests.ArgentRoseStoreForTesting;

namespace ArgentRose.Tests;

public class ArgentRoseStoreTest
{
    private const int MinQuality = 0;
    private const int MaxQuality = 50;
    private const int SellInLastDay = 0;
    private const int Expired = -1;

    [Test]
    public void Regular_Product_Decreases_Quality_By_Two()
    {
        var store = StoreIncluding(RegularProduct(1, 10));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(RegularProduct(0, 8))));
    }

    [Test]
    public void Expired_Regular_Product_Decreases_Quality_Twice_As_Fast()
    {
        var store = StoreIncluding(RegularProduct(SellInLastDay, 10));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(RegularProduct(Expired, 6))));
    }

    [Test]
    public void Lanzarote_Wine_Increases_Quality_By_One()
    {
        var store = StoreIncluding(LanzaroteWine(3, 10));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(LanzaroteWine(2, 12))));
    }

    [Test]
    public void Expired_Lanzarote_Wine_Increases_Quality_Twice_As_Fast()
    {
        var store = StoreIncluding(LanzaroteWine(SellInLastDay, 10));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(LanzaroteWine(Expired, 14))));
    }

    [Test]
    public void Theatre_Passes_Increase_Quality_By_One_When_Sell_In_Is_Far_Away()
    {
        var store = StoreIncluding(TheatrePasses(6, 12));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(TheatrePasses(5, 13))));
    }

    [Test]
    public void Theatre_Passes_Increase_Quality_By_Three_When_Sell_In_Is_Near()
    {
        var store = StoreIncluding(TheatrePasses(5, 12));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(TheatrePasses(4, 15))));
    }

    [Test]
    public void Expired_Theatre_Passes_Drop_Quality_To_Zero()
    {
        var store = StoreIncluding(TheatrePasses(SellInLastDay, 5));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(TheatrePasses(Expired, MinQuality))));
    }

    [Test]
    public void Regular_Product_Quality_Is_Never_Below_The_Minimum_Quality()
    {
        var store = StoreIncluding(RegularProduct(2, 1));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(RegularProduct(1, MinQuality))));
    }

    [Test]
    public void Expired_Regular_Product_Quality_Is_Never_Below_The_Minimum_Quality()
    {
        var store = StoreIncluding(RegularProduct(SellInLastDay, 3));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(RegularProduct(Expired, MinQuality))));
    }

    [Test]
    public void Lanzarote_Wine_Quality_Never_Increases_Over_The_Maximum_Quality()
    {
        var store = StoreIncluding(LanzaroteWine(3, MaxQuality));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(LanzaroteWine(2, MaxQuality))));
    }

    [Test]
    public void Expired_Lanzarote_Wine_Quality_Never_Increases_Over_The_Maximum_Quality()
    {
        var store = StoreIncluding(LanzaroteWine(SellInLastDay, MaxQuality));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(LanzaroteWine(Expired, MaxQuality))));
    }

    [Test]
    public void Theatre_Pass_Quality_When_Sell_In_Is_Far_Away_Never_Increases_Over_The_Maximum_Quality()
    {
        var store = StoreIncluding(TheatrePasses(9, MaxQuality));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(TheatrePasses(8, MaxQuality))));
    }

    [Test]
    public void Theatre_Pass_Quality_When_Sell_In_Is_Near_Never_Increases_Over_The_Maximum_Quality()
    {
        var store = StoreIncluding(TheatrePasses(2, 48));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(InventoryIncluding(TheatrePasses(1, MaxQuality))));
    }

    [Test]
    public void Updating_Multiple_Products()
    {
        var store = StoreIncluding(RegularProduct(10, 4), RegularProduct(3, 5));

        store.Update();

        Assert.That(
            store.SavedInventory,
            Is.EqualTo(
                InventoryIncluding(
                    RegularProduct(9, 2), RegularProduct(2, 3))));
    }

    private static Product RegularProduct(int sellIn, int quality)
    {
        return new Product("RegularProduct", sellIn, quality);
    }

    private static Product LanzaroteWine(int sellIn, int quality)
    {
        return new Product("Lanzarote Wine", sellIn, quality);
    }

    private static Product TheatrePasses(int sellIn, int quality)
    {
        return new Product("Theatre Passes", sellIn, quality);
    }

    private static List<Product> InventoryIncluding(params Product[] products)
    {
        return products.ToList();
    }
}

public class ArgentRoseStoreForTesting : ArgentRoseStore
{
    private List<Product> _savedInventory;
    private readonly List<Product> _initialInventory;

    public static ArgentRoseStoreForTesting StoreIncluding(params Product[] products)
    {
        return new ArgentRoseStoreForTesting(products.ToList());
    }

    private ArgentRoseStoreForTesting(List<Product> initialInventory)
    {
        _savedInventory = new List<Product>();
        _initialInventory = initialInventory;
    }

    protected override List<Product> GetInventoryFromDb()
    {
        return _initialInventory;
    }

    protected override void SaveInventory(List<Product> inventory)
    {
        _savedInventory = inventory;
    }

    public List<Product> SavedInventory => _savedInventory;
}