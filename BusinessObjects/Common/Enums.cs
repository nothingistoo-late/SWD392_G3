namespace BusinessObjects.Common
{
    public enum GenderEnums
    {
        Other,
        MALE,
        FEMALE
    }
    public enum MemberShipType
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Done,
        Cancelled
    }

    public enum OrderDetailStatus
    {
        Pending = 0,
        Confirmed = 1,
        InProgress = 2,
        Completed = 3,
        Cancelled = 4
    }


}
