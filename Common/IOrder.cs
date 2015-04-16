namespace Common
{
    public interface IOrder
    {
        int Wanted { get; set; }
        int Satisfied { get; set; }
        int IdOrder { get; set; }
        int IdUser { get; set; }
        OrderOptionEnum OrderType { get; set; }
    }

    public enum OrderOptionEnum
    {
        Sell,
        Buy
    }
}