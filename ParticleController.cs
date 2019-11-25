using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{

    private bool readyToShow = true;
    private static bool _isShowing;
    private ParticleSystem particles;
    private LevelGenerator levelGenerator;
    private GameObject player;

    void Start()
    {
        particles = GetComponent<ParticleSystem>();
        levelGenerator = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelGenerator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (readyToShow && !PointerController.isShowing())
            StartCoroutine(StartCooldown());
        _isShowing = particles.isPlaying;
    }

    void LateUpdate()
    {
        gameObject.transform.position = player.transform.position + new Vector3(1f, 0);
    }

    public static bool isShowing()
    {
        return _isShowing;
    }

    public float ForceShowParticles()
    {
        if (!_isShowing)
            particles.Play();
        return particles.main.duration;
    }

    private IEnumerator StartCooldown()
    {
        readyToShow = false;
        float cooldown = GameData.getParticleCooldown();
        yield return new WaitForSeconds(cooldown);
        StartCoroutine(ShowParticles());
    }

    private IEnumerator ShowParticles()
    {
        particles.Play();
        yield return new WaitForSeconds(particles.main.duration);
        //levelGenerator.WorldRotate(90f);
        levelGenerator.RandomWorldRotate();
        yield return new WaitForSeconds(particles.main.startLifetimeMultiplier);
        readyToShow = true;
    }

}
