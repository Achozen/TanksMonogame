using Microsoft.Xna.Framework;


namespace MonoGame_SimpleSample
{
    public interface TankActionListener
    {
        void OnFire(int playerIndex, Vector2 position, WalkingDirection walkingDirection);
    }
}