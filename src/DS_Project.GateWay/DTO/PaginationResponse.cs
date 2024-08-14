namespace DS_Project.GateWay.DTO
{
    public class PaginationResponse
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalElements { get; set; }

        public IEnumerable<CarResponse> Items { get; set; }
    }
}
