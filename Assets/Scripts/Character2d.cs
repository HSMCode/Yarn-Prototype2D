using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Character2d : MonoBehaviour {
    [System.Serializable] // map of textures for facial expressions
    
    public class Expression2D {
        public string name;
        public Sprite sprite2D;
    }

    // // objects needed to render new textures on Character face
    [SerializeField] List<Expression2D> expressions2D = new List<Expression2D>();
    private SpriteRenderer _faceRenderer2D;

    // object needed to set Character pose
    private Animator _animator;

    // when this character is first created
    public void Awake() {
        // set initial pose and facial expression to defaults
        _animator = GetComponentInChildren<Animator>();
        _faceRenderer2D = GetComponentInChildren<SpriteRenderer>();
        if (expressions2D.Count < 1) {
            Debug.LogError($"Character {name} has no available facial 2d sprites.");
            return;
        }
        
        SetFaceTexture2D(expressions2D[0].sprite2D);
        Debug.Log($"Character {name} created.");
    }

    // moves character to location {location}>{markerName} in the scene
    [YarnCommand("move")]
    public void Move(Location location, string markerName) {
        Transform destination = location.GetMarkerWithName(markerName);
        if (destination != null) {
            transform.position = destination.position;
            transform.rotation = destination.rotation;
            Debug.Log($"Character {name} moved to {location.name}>{markerName}.");
        }
    }

    // tells the animator to jump to state {poseName} 
    [YarnCommand("triggerAnimation")]
    public void SetAnimtionToTrigger(string poseName) {
        _animator.SetTrigger(poseName);
        Debug.Log($"{name} adopting {poseName} pose.");
    }
    
    // tells the animator to jump to state {poseName} 
    [YarnCommand("playAnimation")]
    public void SetAnimationToPlay(string poseName) {
        _animator.Play("Base Layer." + poseName, 0);
        Debug.Log($"{name} adopting {poseName} pose.");
    }

    // expressions for 2d elements
    // sets character expression texture to {expressionName} texture
    [YarnCommand("expression")]
    public void SetExpression2D(string expressionName2D){
        // find the expression with the same name as we are looking for
        Expression2D expressionToUse2D = FindExpressionWithName2D(expressionName2D);
        if (expressionToUse2D == null) {
            Debug.LogError($"Character {name} has no Expression named {expressionName2D}.");
            return;
        }
        SetFaceTexture2D(expressionToUse2D.sprite2D);
    }
    
    private Expression2D FindExpressionWithName2D(string expressionName2D) {
        var caseInsensitiveMode = System.StringComparison.InvariantCultureIgnoreCase;
        foreach (Expression2D expression2D in expressions2D) {
            if (expression2D.name.Equals(expressionName2D, caseInsensitiveMode)) {
                return expression2D;
            }
        }
        return null;
    }

    private void SetFaceTexture2D(Sprite sprite2D) {
        _faceRenderer2D.sprite = sprite2D;
    }
}