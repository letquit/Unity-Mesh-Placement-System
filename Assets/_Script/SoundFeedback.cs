using UnityEngine;

/// <summary>
/// 音效反馈系统：负责根据游戏事件播放对应的音效。
/// 使用 PlayOneShot 实现音效的快速重叠播放。
/// </summary>
public class SoundFeedback : MonoBehaviour
{
    // 定义具体的音效资源，在 Inspector 中拖入赋值
    [SerializeField] private AudioClip clickSound;       // 菜单点击或模式切换音效
    [SerializeField] private AudioClip placeSound;       // 成功放置建筑音效
    [SerializeField] private AudioClip removeSound;      // 成功拆除建筑音效
    [SerializeField] private AudioClip wrongPlacementSound; // 放置非法（撞墙/重叠）音效
    
    [SerializeField] private AudioSource audioSource;    // 音频源组件，负责发声

    /// <summary>
    /// 根据音效类型播放对应的声音。
    /// </summary>
    /// <param name="soundType">音效类型枚举</param>
    public void PlaySound(SoundType soundType)
    {
        // 安全检查：防止因忘记赋值 Audio Source 导致崩溃
        if (audioSource == null) return;

        switch (soundType)
        {
            case SoundType.Click:
                audioSource.PlayOneShot(clickSound);
                break;
            case SoundType.Place:
                audioSource.PlayOneShot(placeSound);
                break;
            case SoundType.Remove:
                audioSource.PlayOneShot(removeSound);
                break;
            case SoundType.WrongPlacement:
                audioSource.PlayOneShot(wrongPlacementSound);
                break;
            default:
                break;
        }
    }
}

/// <summary>
/// 音效类型枚举：定义游戏中所有与建造相关的音效类别。
/// 使用枚举代替字符串，避免拼写错误，提高代码可读性。
/// </summary>
public enum SoundType
{
    Click,          // 点击/切换
    Place,          // 放置成功
    Remove,         // 拆除成功
    WrongPlacement  // 错误/非法位置
}