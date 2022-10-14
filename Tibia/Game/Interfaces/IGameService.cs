using Tibia.Adventures;

namespace Tibia.Game.Interfaces;

public interface IGameService
{
    bool StartGame(Adventure adventure = null);
}