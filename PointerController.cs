using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerController : MonoBehaviour
{
    private Vector2 axis = new Vector2(0, 1f);
    private Vector2 direction;
    private AudioSource source;
    private bool readyToShow = true;
    private static bool _isShowing;
    private GameObject pointerObject;

    void Start()
    {
        source = GetComponent<AudioSource>();
        pointerObject = gameObject.transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (readyToShow && !ParticleController.isShowing())
            StartCoroutine(StartCooldown());
        _isShowing = pointerObject.activeSelf;
    }

    void LateUpdate()
    {
        direction = GameObject.FindGameObjectWithTag("Finish").transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;
        gameObject.transform.rotation = Quaternion.FromToRotation(axis, direction);

        // Debug.Log("Zero-finish angle: " + Vector2.Angle(axis, direction));
    }

    public static bool isShowing()
    {
        return _isShowing;
    }

    private IEnumerator StartCooldown()
    {
        readyToShow = false;
        float cooldown = GameData.getHintCooldown();
        yield return new WaitForSeconds(cooldown);
        StartCoroutine(ShowHint());
    }

    private IEnumerator ShowHint()
    {
        source.Play();
        pointerObject.SetActive(true);
        yield return new WaitForSeconds(source.clip.length);
        pointerObject.SetActive(false);
        readyToShow = true;
    }
}
