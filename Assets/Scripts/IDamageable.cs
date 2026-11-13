/// <summary>
/// Defines an object that can receive damage. OOP principle: abstraction through an interface.
/// </summary>
public interface IDamageable
{
    // Applies the given amount of damage. OOP principle: polymorphism through a shared contract.
    void TakeDamage(float amount);
}
