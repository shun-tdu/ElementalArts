using UnityEngine;
using MyMaterials.Scripts.Weapon.Behavior.Projectile.Impact;
using MyMaterials.Scripts.Element;


namespace MyMaterials.Scripts.Weapon.Behavior.Projectile.Core
{
    [RequireComponent(typeof(Rigidbody))]
    public class BaseProjectile : MonoBehaviour, IElementReactable
    {
        [Header("Projectile Property")] public ElementType projectileType;

        [SerializeField] private ReactionMatrix reactionMatrix;

        protected Rigidbody Rb;
        private Impact.IProjectileImpact _impact;
        
        //IElementReactableのプロパティ
        public ElementType ElementType => projectileType;

        /// <summary>
        /// 他の属性から作用された時の、この弾自身の固有の反応を定義します。
        /// virtualにしておくことで、派生クラスでこの挙動を上書き（override）できます。
        /// </summary>
        /// <returns>固有の反応を処理した場合はtrue、そうでなければfalse</returns>
        public virtual bool ReactTo(ElementType incomingType, Vector3 hitPoint, Vector3 hitDirection)
        {
            return false;
        }
        
        private void Awake() => Rb = GetComponent<Rigidbody>();

        /// <summary>
        /// 衝突時の挙動を設定
        /// </summary>
        public virtual void Initialize(IProjectileImpact impact)
        {
            _impact = impact;
        }


        /// <summary>
        /// 初速度を設定
        /// </summary>
        public void SetInitialVelocity(Vector3 velocity)
        {
            Rb.velocity = velocity;
        }

        /// <summary>
        /// 衝突時の処理をImpactInterfaceに委譲後、Projectileを削除
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // Debug.Log($"衝突発生！ 相手: {other.gameObject.name}");

            //エレメント反応処理
            var reactable = other.GetComponent<IElementReactable>();
            if (reactable != null)
            {
                //衝突対象の固有の反応を処理
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                bool handled = reactable.ReactTo(this.ElementType, hitPoint, transform.forward);

                //固有の反応がない場合は共通のエレメント反応を処理
                if (!handled && reactionMatrix != null)
                {
                    GameObject reactionEffect =
                        reactionMatrix.GetReactionEffect(this.ElementType, reactable.ElementType);
                    if (reactionEffect != null)
                    {
                        Vector3 reactionPosition = (transform.position + other.transform.position) / 2f;
                        Instantiate(reactionEffect, reactionPosition, Quaternion.identity);

                        if (other.GetComponent<BaseProjectile>() != null) Destroy(other.gameObject);
                        Destroy(gameObject);
                        return;
                    }
                }
            }

            //Projectileの衝突時の処理呼び出し
            _impact?.OnImpact(transform, other);
            Destroy(gameObject);

            //todo 最強の解決策】ハイブリッド（併用）アプローチを実装する
        }
    }
}