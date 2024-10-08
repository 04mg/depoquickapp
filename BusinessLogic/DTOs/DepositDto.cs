namespace BusinessLogic.DTOs;

public struct DepositDto
{
    public string Name { set; get; }
    public string Area { get; set; }
    public string Size { get; set; }
    public bool ClimateControl { get; set; }
    public List<PromotionDto> Promotions { get; set; }

    public List<DateRangeDto> AvailabilityPeriods { get; set; }
}