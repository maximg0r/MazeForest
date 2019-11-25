using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    public GameObject winScreen;
    public ParticleController particleController;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.touchCount == 0)
            animator.SetBool("IsMoving", false);
        else
            foreach (Touch touch in Input.touches)
            {
                HandleTouch(touch);
            }

        #region touchSimulation
        // simulate touches in unity editor

        if (Input.GetMouseButton(0))
        {
            Touch fakeTouch = new Touch();
            fakeTouch.fingerId = 10;
            fakeTouch.position = Input.mousePosition;
            fakeTouch.deltaTime = Time.deltaTime;
            fakeTouch.deltaPosition = fakeTouch.position;
            fakeTouch.phase = (Input.GetMouseButtonDown(0) ? TouchPhase.Began :
                                (fakeTouch.deltaPosition.sqrMagnitude > 1f ? TouchPhase.Moved : TouchPhase.Stationary));
            fakeTouch.tapCount = 1;

            HandleTouch(fakeTouch);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("IsMoving", false);
        }

        #endregion
    }

    void HandleTouch(Touch touch)
    {
        animator.SetBool("IsMoving", true);

        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(touch.position); // get world point from touch coordinates
        Vector3 direction = worldPoint - gameObject.transform.position; // get direction for player
        direction.z = 0;

        gameObject.transform.Translate(direction * Time.deltaTime * moveSpeed); // move the player

        // change the player's orientation
        if (direction.x > 0)
            gameObject.transform.localScale = new Vector3(-0.5f, 0.5f, 1);
        else
            gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Border":
                //Debug.Log("Reached world border. Translating to spawn");
                StartCoroutine(TranslateToSpawn());
                break;
            case "Finish":
                //Debug.Log("Level complete. Restarting game");
                winScreen.SetActive(true);
                Time.timeScale = 0;
                break;
        }
    }

    private IEnumerator TranslateToSpawn()
    {
        yield return new WaitForSeconds(particleController.ForceShowParticles());
        transform.Translate((new Vector3(0, 0, 0) - transform.position) * 0.25f);
    }
}
