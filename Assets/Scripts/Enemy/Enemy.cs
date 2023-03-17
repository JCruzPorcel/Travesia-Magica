using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Range(0f, 50f)][SerializeField] protected float speed = 10f;


    private void Update()
    {
        DespawnDistance();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.currentGameState == GameState.InGame)
        Movement();
    }

    public virtual void Movement()
    {
        transform.position += Vector3.left * speed * Time.fixedDeltaTime;
    }

    private void DespawnDistance()
    {
        if (transform.position.x <= -50f)
        {
            this.gameObject.SetActive(false);
        }
    }
}
