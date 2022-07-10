using Dislinkt.Saga.Data;

namespace Dislinkt.Saga.Menager
{
    public interface IRegistrationMenager
    {
        public bool Register(UserData input);
    }
}
