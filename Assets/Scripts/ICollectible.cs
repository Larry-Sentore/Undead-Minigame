/// <summary>
/// Defines an object the player can collect. OOP principle: abstraction through an interface.
/// </summary>
public interface ICollectible
{
    // Gives this collectible's reward to the player. OOP principle: polymorphism through a shared contract.
    void Collect(PlayerController player);
}
