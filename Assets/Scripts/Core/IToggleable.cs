// Implemented by any hazard/object that a HoldPlate can switch on/off
// (StaticLaser, Laser, etc.) — lets HoldPlate stay generic instead of
// hardcoding a check for each hazard type it might be linked to.

public interface IToggleable
{
    void SetActive(bool active);
}
