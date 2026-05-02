using UnityEngine;

public class SoundFeedback : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip removeSound;
    [SerializeField] private AudioClip wrongPlacementSound;
    
    [SerializeField] private AudioSource audioSource;

    public void PlaySound(SoundType soundType)
    {
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

public enum SoundType
{
    Click,
    Place,
    Remove,
    WrongPlacement
}
