using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string objectName;



    public string ObjectName { get; set; }


    //ToDo: Implementar atributos de los Items.
}
