namespace Common
{
    public interface IOrder
    {
        string Date { get; set; }
        int Wanted { get; set; }
        int Satisfied { get; set; }
        int IdOrder { get; set; }
        int IdUser { get; set; }
        bool Closed { get; set; }
        float SharePrice { get; set; }
        OrderOptionEnum OrderType { get; set; }
    }

    public enum OrderOptionEnum
    {
        Sell,
        Buy
    }
}