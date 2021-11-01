using TMPro;
using Turrets.Blueprints;
using UnityEngine;
using UnityEngine.UI;
using Turrets;

public class TurretSelectionUI : MonoBehaviour
{
    private TurretBlueprint _turretBlueprint;
    
    // Content
    public TextMeshProUGUI displayName;
    public TextMeshProUGUI tagline;

    public Image icon;

    public TextMeshProUGUI noneText;

    [Header("Stats")]
    public TurretStat damage;
    public TurretStat rate;
    public TurretStat range;

    [Header("Colors")]
    public Image bg;
    public Image upgradesBG;
    public TextMeshProUGUI upgradesTitle;

    // Called when creating the UI
    public void Init (TurretBlueprint turret, Shop shop)
    {
        _turretBlueprint = turret;

        displayName.text = turret.displayName;
        tagline.text = turret.tagline;

        icon.sprite = turret.shopIcon;

        Turret turretPrefab = turret.prefab.GetComponent<Turret>();
        switch (turretPrefab.attackType)
        {
            case TurretType.Bullet:
                damage.SetData(
                    turretPrefab.bulletPrefab.GetComponent<Bullet>().damage
                    );
                break;

            case TurretType.Laser:
                damage.SetData(turretPrefab.damageOverTime);
                break;

            case TurretType.Area:
                damage.SetData(turretPrefab.smashDamage);
                break;
        }
        rate.SetData(turretPrefab.fireRate);
        range.SetData(turretPrefab.range);

        // TODO - Display Upgrades

        // Colors
        tagline.color = turret.accent;
        upgradesTitle.color = turret.accent;
        bg.color = turret.accent;
        upgradesBG.color = turret.accent * new Color(1, 1, 1, .16f);

        damage.SetColor(turret.accent);
        rate.SetColor(turret.accent);
        range.SetColor(turret.accent);

        bg.GetComponent<Button>().onClick.AddListener(delegate { MakeSelection(shop); });
    }

    // Called when the user clicks on the button
    private void MakeSelection (Shop shop)
    {
        transform.parent.gameObject.SetActive (false);
        Time.timeScale = 1f;
        
        shop.SpawnNewTurret(_turretBlueprint);
    }
}
