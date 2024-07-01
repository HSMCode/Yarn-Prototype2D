using UnityEngine;
using Yarn.Unity;

public class SceneDirector : MonoBehaviour {
    private DialogueRunner dialogueRunner; // utility object that serves lines of dialogue
    private FadeLayer fadeLayer; // black overlay used to fade in/out of scenes
    
    [System.Serializable] 
    public class SFXAudioItem
    {
        public string audioName;
        public AudioClip audioClip;
    }

    public SFXAudioItem[] audioItems;
    private AudioSource _audioSource;
    
    
    
    [System.Serializable] 
    public class BackgroundElements
    {
        public string backgroundName;
        public Sprite backgroundSprite;
    }
    
    public BackgroundElements[] backgroundElements;
    // public GameObject backgroundPosition;
    private SpriteRenderer _backgroundPositionRenderer;
    
    
    // when this scene conductor object is created
    // (in our example, this happens when the scene is created)
    private void Awake() {
        // get handles of utility objects in the scene that we need
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        fadeLayer = FindObjectOfType<FadeLayer>();
        
        // grab the Audio Source component from the game object
        _audioSource = GetComponent<AudioSource>();

        // set the Backdrop Background as Default Background Renderer
        _backgroundPositionRenderer = GameObject.Find("Backdrop").GetComponentInChildren<SpriteRenderer>();
        //grab the sprite renderer on our position object
        // _backgroundPositionRenderer = backgroundPosition.GetComponent<SpriteRenderer>();

        // register Command Handler for <<camera NAME_OF_LOCATION>>
        dialogueRunner.AddCommandHandler<Location>("camera", MoveCamera);

        // Handlers for <<fadeIn DURATION>> and <<fadeOut DURATION>>
        dialogueRunner.AddCommandHandler<float>("fadeIn", FadeIn);
        dialogueRunner.AddCommandHandler<float>("fadeOut", FadeOut);
        
        // Handler to play AudioSFX
        dialogueRunner.AddCommandHandler<string>("playSFX", PlaySFXAudio);
        
        // Handler to swap Backgrounds
        dialogueRunner.AddCommandHandler<string>("swapBackground", BackgroundSwap);

        Debug.Log("SceneConductor created.");
    }

    // moves camera to camera location {location}>Camera in the scene
    private void MoveCamera(Location location) {
        Transform destination = location.GetMarkerWithName("Camera");
        if (destination != null) {
            Camera.main.transform.position = destination.position;
            Camera.main.transform.rotation = destination.rotation;
            Debug.Log($"Main Camera moved to {location.name}>Camera.");
        }
    }

    // fades in from a black screen over {time} seconds
    private Coroutine FadeIn(float time = 1f) {
        Debug.Log($"Fading in from black over {time} seconds.");
        return StartCoroutine(fadeLayer.ChangeAlphaOverTime(0, time));
    }

    // fades out to a black screen over {time} seconds
    private Coroutine FadeOut(float time = 1f) {
        Debug.Log($"Fading out to black over {time} seconds.");
        return StartCoroutine(fadeLayer.ChangeAlphaOverTime(1, time));
    }
    
    // play audio sfx called by name, setup in Unity Editor
    private void PlaySFXAudio(string clipName)
    {
        foreach (SFXAudioItem item in audioItems)
        {
            if (item.audioName != clipName) continue;
                _audioSource.PlayOneShot(item.audioClip, 1f);
                return; 
        }
        Debug.LogWarning($"couldn't find {clipName} audio");
    }
    
    
    // swap backgrounds on the background game object
    private void BackgroundSwap(string backgroundNamed)
    {
        foreach (BackgroundElements element in backgroundElements)
        {
            if (element.backgroundName != backgroundNamed) continue;
                SetBackgroundName(element.backgroundSprite);
                Debug.Log($"Changed background to {backgroundNamed}.");
                return; 
        }
        Debug.LogWarning($"couldn't find {backgroundNamed} background");
    }
    
    private void SetBackgroundName(Sprite backgroundSprite) {
        _backgroundPositionRenderer.sprite = backgroundSprite;
    }
}