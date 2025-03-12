using UnityEngine;

namespace WwiseHelper
{
    public class WwiseAudioManager : MonoBehaviour
    {
#if WWISE_2024_OR_LATER
        public string[] playerPrefsNames;
        //this is so the method is run as soon as the scene starts
        private void Start()
        {
            LoadPlayerPrefs();
        }

        //this method simple loads the player preferences based on the name
        private void LoadPlayerPrefs()
        {
            foreach (var pref in playerPrefsNames)
            {
                if (PlayerPrefs.HasKey(pref))
                    AkUnitySoundEngine.SetRTPCValue(pref, PlayerPrefs.GetFloat(pref));
                else
                {
                    //if the key does not exist, set it to 0.8 and save it
                    PlayerPrefs.SetFloat(pref, 0.8f);
                    AkUnitySoundEngine.SetRTPCValue(pref, 0.8f);
                }
            }
        }
#endif
    }
}
