using UnityEngine;

namespace Element
{
    /// <summary>
    /// エレメントを持つEntityが持つインターフェイス
    /// オブジェクト固有のエレメント反応を持つ場合は固有の反応を発生
    /// そうでない場合は、一般的なエレメント反応を発生
    /// </summary>
    public interface IElementReactable
    {
        /// <summary>
        /// 自身のエレメントを返すプロパティ
        /// </summary>
        
        ElementType ElementType { get; }
        /// <summary>
        /// 別の属性からの作用に反応するメソッド
        /// </summary>
        /// <param name="incomingType">作用してきた相手の属性</param>
        /// <param name="hitPoint">作用点</param>
        /// <param name="hitDirection">作用方向</param>
        /// <returns>固有の反応を処理した場合はtrue、そうでない場合はfalse</returns>
        bool ReactTo(ElementType incomingType, Vector3 hitPoint, Vector3 hitDirection);
    }
}