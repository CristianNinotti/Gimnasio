using Domain.Entities.Enum;

namespace Domain.Entities
{
    public class Customer : User
    {
        public Customer()
        {
            UserType = UserType.Customer;
        }
    }
}