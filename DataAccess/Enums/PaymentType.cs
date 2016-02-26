
namespace DataAccess.Enums
{
    public enum PaymentType
    {
        /// <summary>
        /// Somebody paid certain amount for somebody else
        /// </summary>
        Debt,

        /// <summary>
        /// Auto-generated transaction which splits the remaining amount to all people in the group
        /// </summary>
        Rounding
    }
}
