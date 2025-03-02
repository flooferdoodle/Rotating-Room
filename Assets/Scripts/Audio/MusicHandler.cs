using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    // Singleton Setup
    #region Singleton
    private static MusicHandler _instance;
    public static MusicHandler Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        drumTracks = new List<AudioSource>();
        instrumentTracks = new List<AudioSource>();
        for (int i = 0; i < dimensionTracks.Count; i++)
        {
            drumTracks.Add(gameObject.AddComponent<AudioSource>());
            instrumentTracks.Add(gameObject.AddComponent<AudioSource>());

            drumTracks[i].clip = dimensionTracks[i].drums;
            drumTracks[i].loop = true;
            drumTracks[i].volume = 0f;
            instrumentTracks[i].clip = dimensionTracks[i].instruments;
            instrumentTracks[i].loop = true;
            instrumentTracks[i].volume = 0f;
        }
    }
    #endregion

    [System.Serializable]
    public struct DimensionMusicTracks
    {
        public DimensionButton dimension;
        public AudioClip drums;
        public AudioClip instruments;
    }
    public List<DimensionMusicTracks> dimensionTracks;
    public float fadeSpeed = 3f;

    private List<AudioSource> drumTracks;
    private List<AudioSource> instrumentTracks;

    private double nextScheduleTime;
    private double loopLength;

    private int activeDrumIndex = -1;
    private int activeInstrumentIndex = -1;

    // Start is called before the first frame update

    void Start()
    {
        

        double currTime = AudioSettings.dspTime;
        double scheduleStart = currTime + 1f;
        for(int i = 0; i < dimensionTracks.Count; i++)
        {
            drumTracks[i].PlayScheduled(scheduleStart);
            instrumentTracks[i].PlayScheduled(scheduleStart);
            //drumTracks[1][i].Play();
            //instrumentTracks[1][i].Play();
        }
        loopLength = dimensionTracks[0].drums.samples / dimensionTracks[0].drums.frequency;

        nextScheduleTime = scheduleStart + loopLength;
    }

    private int GetDimensionIndex(DimensionButton dim)
    {
        for(int i = 0; i < dimensionTracks.Count; i++)
        {
            if (dimensionTracks[i].dimension == dim) return i;
        }
        return -1;
    }

    public void PlayDimensionTrack(DimensionButton dim, bool drumOrInstruments)
    {
        int i = GetDimensionIndex(dim);
        if (i == -1)
        {
            Debug.Log("No music tracks found for dimension.");
        }
        if (drumOrInstruments)
        {
            if (activeDrumIndex > -1) StartCoroutine(FadeTrackOut(drumTracks[activeDrumIndex]));
            StartCoroutine(FadeTrackIn(drumTracks[i]));
            activeDrumIndex = i;
        }
        else
        {
            if (activeInstrumentIndex > -1) StartCoroutine(FadeTrackOut(instrumentTracks[activeInstrumentIndex]));
            StartCoroutine(FadeTrackIn(instrumentTracks[i]));
            activeInstrumentIndex = i;
        }
    }
    private IEnumerator FadeTrackIn(AudioSource source)
    {
        while(source.volume < 1f)
        {
            source.volume += fadeSpeed * 0.01f;
            yield return null;
        }
        source.volume = 1f;
    }
    private IEnumerator FadeTrackOut(AudioSource source)
    {
        while (source.volume > 0f)
        {
            source.volume -= fadeSpeed * 0.01f;
            yield return null;
        }
        source.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
        /*double time = AudioSettings.dspTime;
        if (time + 1.0f > nextScheduleTime)
        {
            for (int i = 0; i < dimensionTracks.Count; i++)
            {
                drumTracks[flip][i].PlayScheduled(nextScheduleTime);
                instrumentTracks[flip][i].PlayScheduled(nextScheduleTime);
            }
            nextScheduleTime = nextScheduleTime + loopLength;
            flip = 1 - flip;
        }*/
    }
}
