namespace DS_Project.GateWay
{
    public class CreateRentalRequest
    {
        public Guid CarUid { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
    }
}
