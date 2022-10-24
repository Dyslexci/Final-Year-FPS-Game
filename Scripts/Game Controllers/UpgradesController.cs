using System.Collections.Generic;
using UnityEngine;

public class UpgradesController : MonoBehaviour
{
    public static UpgradesController instance;
    public List<string> upgradeCategories;
    private void Awake()
    {
        instance = this;
    }
}
