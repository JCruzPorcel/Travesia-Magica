using UnityEngine;

public class ItemBase : MonoBehaviour
{
    [Range(0f, 50f)][SerializeField] protected float speed = 1f;


    private void Update()
    {
        DespawnDistance();
    }

    private void FixedUpdate()
    {
        ObjectMovement();
    }


    public virtual void ObjectMovement()
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


    public virtual void ObjectAtribute() { }
}
