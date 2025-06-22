using UnityEngine;
using Player;
using UnityEngine.UI;

namespace UI
{
    public class OffScreenIndicator : MonoBehaviour
    {
        [SerializeField] private Image arrowImage;

        [SerializeField] private Canvas canvas;

        // [SerializeField] private Camera mainCamera;
        [SerializeField] private LockOnManager lockOnManager;

        [SerializeField, Tooltip("端からのマージン(pix)")]
        private float edgeMargin = 50f;

        private RectTransform canvasRect;
        private Camera mainCamera;

        private void Awake()
        {
            canvasRect = canvas.GetComponent<RectTransform>();
            mainCamera = Camera.main;
        }

        private void Update()
        {
            var target = lockOnManager.GetCurrentTarget();
            if (target == null)
            {
                // 以前のコードでImage.enabledを使っているならこちら
                if (arrowImage != null) arrowImage.enabled = false;
                return;
            }

            // ターゲットのワールド座標をスクリーン座標(ピクセル)に変換
            Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

            // ターゲットが画面内にいるかを判定 (マージンを少し考慮)
            bool onScreen = screenPos.z > 0 &&
                            screenPos.x > edgeMargin && screenPos.x < Screen.width - edgeMargin &&
                            screenPos.y > edgeMargin && screenPos.y < Screen.height - edgeMargin;

            if (onScreen)
            {
                // 画面内にいるなら非表示
                if (arrowImage != null) arrowImage.enabled = false;
                return;
            }

            // 画面外にいるなら表示
            if (arrowImage != null) arrowImage.enabled = true;

            // ターゲットがカメラの後ろにいる場合、座標を反転させる
            if (screenPos.z < 0)
            {
                // 最も挙動が安定する方法です
                screenPos *= -1;
            }

            // 画面の中心座標
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

            // 画面中心からターゲットのスクリーン座標への方向ベクトル
            Vector2 direction = new Vector2(screenPos.x, screenPos.y) - screenCenter;

            // 画面の境界線（マージンを考慮した内側の枠）までの距離を計算
            float boundsX = (Screen.width - edgeMargin * 2) * 0.5f;
            float boundsY = (Screen.height - edgeMargin * 2) * 0.5f;

            // 方向ベクトルが画面の境界をどれだけはみ出しているかの比率(scale)を計算
            // X方向とY方向で、よりはみ出している方を基準にする
            float scale = Mathf.Max(Mathf.Abs(direction.x) / boundsX, Mathf.Abs(direction.y) / boundsY);

            // scaleが1より大きい場合（＝画面外の場合）、ベクトルを縮小して境界線上に乗るようにする
            if (scale > 1f)
            {
                direction /= scale;
            }

            // 最終的なインジケーターのスクリーン座標を計算
            Vector2 indicatorScreenPosition = screenCenter + direction;

            // UIの位置を更新
            // この方法は、CanvasのRenderModeやScalerの設定によらず安定します
            arrowImage.rectTransform.position = indicatorScreenPosition;

            // 向きを計算
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            arrowImage.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }
}

//
// var target = lockOnManager.GetCurrentTarget();
// Debug.Log($"[OffScr] Target={target}");
// if (target == null)
// {
//     arrow.gameObject.SetActive(false);
//     return;
// }
//
// // ワールド→スクリーン
// Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
// bool onScreen = screenPos.z > 0
//              && screenPos.x >= 0 && screenPos.x <= Screen.width
//              && screenPos.y >= 0 && screenPos.y <= Screen.height;
// // Debug.Log($"[OffScr] screenPos={screenPos} onScreen={onScreen}");
//
// Debug.Log($"[OffScr] screenPos={screenPos} onScreen={onScreen}");
// if (onScreen)
// {
//     arrow.gameObject.SetActive(false);
//     return;
// }
//
// // オフスクリーン時
// arrow.gameObject.SetActive(true);
//
// // マージン内にクランプ
// screenPos.x = Mathf.Clamp(screenPos.x, edgeMargin, Screen.width  - edgeMargin);
// screenPos.y = Mathf.Clamp(screenPos.y, edgeMargin, Screen.height - edgeMargin);
//
// // スクリーン→Canvasローカル
// RectTransformUtility.ScreenPointToLocalPointInRectangle(
//     canvasRect,
//     screenPos,
//     canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
//     out Vector2 localPoint
// );
// arrow.anchoredPosition = localPoint;
//
// // 向き
// Vector2 screenCenter = new Vector2(Screen.width, Screen.height) * 0.5f;
// Vector2 dir = new Vector2(screenPos.x, screenPos.y) - screenCenter;
// float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
// arrow.localRotation = Quaternion.Euler(0, 0, angle);