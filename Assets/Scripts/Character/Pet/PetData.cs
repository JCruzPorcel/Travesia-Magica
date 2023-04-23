using UnityEngine;

[CreateAssetMenu(fileName = "New Pet", menuName = "Pet")]
public class PetData : ScriptableObject
{
    [SerializeField] private string petName;
    [SerializeField] private Sprite petImage;
    [Space(10)]
    [SerializeField] private GameObject prefab;

    public string PetName { get { return petName; } }
    public Sprite PetImage { get { return petImage; } }
    public GameObject Prefab { get { return prefab; } }

}
