using Domain.Entities.Enum;

namespace Domain.Entities
{
    public class SuperAdmin : User
    {
        public SuperAdmin()
        {
            UserType = UserType.SuperAdmin;
        }
    }
}