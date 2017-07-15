namespace NumeralCalculator
{
    public interface IProductCategorizer
    {
        bool IsProductIntergalactic(string product);

        bool IsProductEarthy(string product);

        bool IsProductValid(string product);

        ProductType GetProductType(string product);
    }
}