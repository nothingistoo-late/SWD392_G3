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
        Cancelled,
        Paid
    }

    public enum OrderDetailStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3
    }


}
