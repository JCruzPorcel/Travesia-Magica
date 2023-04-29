using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField] float speed;

    public float Speed { get => speed; set => speed = value; }

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
            this.gameObject.SetActive(false);
        }
    }
}
