using Meowrio.Domain;

namespace Meowrio.Service
{
    public interface IEffectable
    {
        public void ApplyEffect(PlayerEntity affectedPlayer);
    }
}