using System.Collections;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("Beam Reference")]
    public GameObject laserBeam;

    [Header("Timing")]
    public float onDuration = 1.5f;
    public float offDuration = 1f;

    private bool laserPermanentlyOff = false;
    private Coroutine blinkRoutine;

    void Start()
    {
        blinkRoutine = StartCoroutine(BlinkLoop());
    }

    IEnumerator BlinkLoop()
    {
        while (!laserPermanentlyOff)
        {
            laserBeam.SetActive(true);
            yield return new WaitForSeconds(onDuration);

            if (laserPermanentlyOff) yield break;

            laserBeam.SetActive(false);
            yield return new WaitForSeconds(offDuration);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (laserPermanentlyOff) return;

        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.Die();
        }
        else if (other.CompareTag("Echo"))
        {
            laserPermanentlyOff = true;

            if (blinkRoutine != null)
                StopCoroutine(blinkRoutine);

            laserBeam.SetActive(false);

            other.GetComponent<EchoPlayer>()?.Despawn();

            Debug.Log("Echo sacrificed itself - laser disabled permanently");
        }
    }
}