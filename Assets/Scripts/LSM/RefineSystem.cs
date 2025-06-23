using UnityEngine;

public class RefineSystem : MonoBehaviour
{
    public MaterialItem ore;
    public MaterialItem ingot;
    public MaterialItem gemOre;
    public Gem gem;
    public MaterialItem strangeStone;

    public string Refine(MaterialItem material)
    {
        if (material.materialType == MaterialType.Ore)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(ingot);
                return "���� ����! ���� �� �ֱ�";
            }
            else return "������ �����մϴ�!";
        }
        else if (material.materialType == MaterialType.GemOre)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(gem);
                return "���� ����! ���� ���� �� ����";
            }
            else return "���� ������ �����մϴ�!";
        }
        else if (material.materialType == MaterialType.StrangeStone)
        {
            if (InventoryManager.Instance.HasEnoughItems(material, 1))
            {
                InventoryManager.Instance.RemoveItem(material);
                InventoryManager.Instance.AddItem(ingot);
                InventoryManager.Instance.AddItem(gem);
                return "�̻��� �� �� �ֱ� & ���� �� �� ȹ��!";
            }
            else return "�̻��� ���� �����մϴ�!";
        }
        else
        {
            return "����/���� �Ұ����� ����Դϴ�.";
        }
    }
}
