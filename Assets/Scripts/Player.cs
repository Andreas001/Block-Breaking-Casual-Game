using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    #region Variables
    [Header("Components")]
    public Rigidbody2D rb;
    public ScreenShake screenShake;
    public GameSession gameSession;

    [Header("Movement")]
    public bool canMove = false;
    [Range(0.05f, 20f)] public float throwForce = 0.3f;

    [Header("Game")]
    public List<Color> colors;
    public Color currentColor;

    [Header("Destroy Effect")]
    public ObjectPool destroyEffectPool;
    public AudioClip destroySfx;
    [Range(0, 1)] public float destroySfxVolume = 0.25f;

    Vector2 startPos, endPos, direction;
    float touchTimeStart, touchTimeFinish, timeInterval;
    #endregion

    #region Unity Callback Functions
    void Awake() {
        if (!rb) {
            rb = GetComponent<Rigidbody2D>();
        }

        if (!screenShake) {
            screenShake = Camera.main.transform.GetComponent<ScreenShake>();
        }

        GetComponent<SpriteRenderer>().color = currentColor;
        destroyEffectPool = GetComponent<ObjectPool>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0) && canMove) {
            touchTimeStart = Time.time;
            startPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && canMove) {
            touchTimeFinish = Time.time;
            timeInterval = touchTimeFinish - touchTimeStart;
            endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = startPos - endPos;

            rb.AddForce(-direction / timeInterval * throwForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        SpriteRenderer sr = collision.transform.GetComponent<SpriteRenderer>();
        if (!sr) {
            return;
        }

        if (collision.transform.GetComponent<SpriteRenderer>().color == currentColor) {
            AudioSource.PlayClipAtPoint(destroySfx, Camera.main.transform.position, destroySfxVolume);

            gameSession.AddScore((int)collision.relativeVelocity.magnitude);
            gameSession.DecreaseAmountOfBreakablesLeft(1);

            screenShake.StartShaking(collision);
            StartCoroutine(SpawnDeathEffect(collision.transform.GetComponent<SpriteRenderer>()));

            collision.gameObject.SetActive(false);
            Vector2 direction = rb.velocity;
            direction.Normalize();
            direction *= rb.velocity.magnitude / 2;
            rb.AddForce(direction);
        }
        else {
            float radiants = 0;
            while (radiants == 0) {
                radiants = Random.Range(0, 2 * Mathf.PI);
            }
            Vector2 direction = new Vector2(Mathf.Cos(radiants), Mathf.Sin(radiants));
            direction.Normalize();
            direction *= rb.velocity.magnitude / 2;
            rb.AddForce(direction);
        }
    }
    #endregion

    #region Functions
    public void RandomizeCurrentColor() {
        currentColor = colors[Random.Range(0, colors.Count)];
        GetComponent<SpriteRenderer>().color = currentColor;
    }

    IEnumerator SpawnDeathEffect(SpriteRenderer sr) {
        GameObject destroyEffect = destroyEffectPool.GetObject();
        ParticleSystem.MainModule settings = destroyEffect.GetComponent<ParticleSystem>().main;
        settings.startColor = new ParticleSystem.MinMaxGradient(sr.GetComponent<SpriteRenderer>().color);
        destroyEffect.transform.position = sr.transform.position;
        destroyEffect.SetActive(true);
        destroyEffect.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2f);
        destroyEffectPool.ReturnObject(destroyEffect);
    }

    public void SetCanMove(bool newBool) {
        this.canMove = newBool;
    }
    #endregion
}
