using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField] Sprite[] clouds;
    [SerializeField] float speed;

    public float Speed { get => speed; set => speed = value; }

    private void Start()
    {
        int randomIndex = Random.Range(0, clouds.Length);
        GetComponent<SpriteRenderer>().sprite = clouds[randomIndex];
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            DespawnDistance();
        }
    }

    private void DespawnDistance()
    {
        if (transform.position.x <= -50)
        {
            gameObject.SetActive(false);
        }
    }
}
