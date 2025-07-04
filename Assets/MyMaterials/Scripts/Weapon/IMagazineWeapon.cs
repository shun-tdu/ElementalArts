namespace MyMaterials.Scripts.Weapon
{
    /// <summary>
    /// マガジンサイズとリロード時間を持つ武器が実装すべきインターフェース
    /// </summary>
    public interface IMagazineWeapon
    {
        int MagazineSize { get; }
        float ReloadTime { get; }
    }
}