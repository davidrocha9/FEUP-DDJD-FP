using UnityEngine;

public class FactoryMusicPlayer : MonoBehaviour
{

    private FMOD.Studio.EventInstance music;

    // Start is called before the first frame update
    void Start()
    {
        music = FMODUnity.RuntimeManager.CreateInstance("event:/Project/Factory Arena/Ambient Music/Music");
        music.start();
        music.release();
    }

    private void OnDestroy()
    {
        music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
