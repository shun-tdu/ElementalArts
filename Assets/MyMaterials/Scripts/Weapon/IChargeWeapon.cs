namespace MyMaterials.Scripts.Weapon
{
    /// <summary>
    /// エネルギー(チャージ)を持つ武器が実装すべきインターフェース
    /// </summary>
    public interface IChargeWeapon
    {
        // 現在のチャージ量はWeaponSystemで管理する
        float MaxCharge { get; }
    }
}