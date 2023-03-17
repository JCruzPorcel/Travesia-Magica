using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterData : ScriptableObject
{
    [SerializeField] private string characterName;
    [SerializeField] private Sprite characterImage;
    [Space(10)]
    [SerializeField] private GameObject prefab;

    public string CharacterName { get { return characterName; } }
    public Sprite CharacterImage { get { return characterImage; } }
    public GameObject Prefab { get { return prefab; } }

}
