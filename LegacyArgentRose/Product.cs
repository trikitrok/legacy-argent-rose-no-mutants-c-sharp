namespace ArgentRose;

public class Product
{
    private const int MinimumQuality = 0;
    private const int MaximumQuality = 50;
    private const int SellInLastDay = 0;
    private const int QualityFactorByExpiration = 2;


    private readonly string _description;
    private int _sellIn;
    private int _quality;

    public Product(string description, int sellIn, int quality)
    {
        _description = description;
        _sellIn = sellIn;
        _quality = quality;
    }

    public void DecreaseQualityBy(int decrement)
    {
        if (IsExpired())
        {
            decrement *= QualityFactorByExpiration;
        }
        _quality = Math.Max(MinimumQuality, _quality - decrement);
    }

    public void IncreaseQualityBy(int increment)
    {
        if (IsExpired())
        {
            increment *= QualityFactorByExpiration;
        }
        _quality = Math.Min(MaximumQuality, _quality + increment);
    }

    public void DropQualityToMinimum()
    {
        _quality = MinimumQuality;
    }

    public void DecreaseSellIn()
    {
        _sellIn -= 1;
    }

    public bool IsExpired()
    {
        return _sellIn < SellInLastDay;
    }

    public bool SellInIsLessThan(int days)
    {
        return _sellIn < days;
    }

    public bool IsLanzaroteWine()
    {
        return _description == "Lanzarote Wine";
    }

    public bool IsTheatrePasses()
    {
        return _description == "Theatre Passes";
    }

    public string Description => _description;

    public int SellIn => _sellIn;

    public int Quality => _quality;

    protected bool Equals(Product other)
    {
        return _description == other._description && _sellIn == other._sellIn && _quality == other._quality;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Product)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_description, _sellIn, _quality);
    }

    public override string ToString()
    {
        return
            $"{nameof(Description)}: {Description}, {nameof(SellIn)}: {SellIn}, {nameof(Quality)}: {Quality}";
    }
}