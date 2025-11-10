namespace HttpChannel.Models
{
    public class ProductResult
    {
        public ProductResult()
        {

            Result= new ProductInfo();
        }
        public int Code { get; set; }
        public string? Msg { get; set; }
        public ProductInfo Result { get; set; }

    }
}
