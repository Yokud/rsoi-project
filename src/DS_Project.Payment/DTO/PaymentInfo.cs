namespace DS_Project.Payments.DTO
{
    public class PaymentInfo
    {
        public Guid PaymentUid { get; set; }
        public string Status { get; set; }
        public int Price { get; set; }
    }
}