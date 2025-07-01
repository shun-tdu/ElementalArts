using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// ダメージを受け取る処理
    /// </summary>
    /// <param name="damage">受けるダメージ量</param>
    /// <param name="hitPoint">ダメージを受けた座標(エフェクト発生などに使用)</param>
    /// <param name="hitDirection">どの方向からダメージを受けたか(ノックバックなどに使う)</param>
    void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection);
}
