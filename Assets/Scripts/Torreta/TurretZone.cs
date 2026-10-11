using UnityEngine;

public class TurretZone : MonoBehaviour
{
    public TurretController turret;

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other)) turret.playerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            turret.playerInZone = false;
            turret.SendMessage("StopAudio");
        }
    }

    private bool IsPlayer(Collider other)
    {
        return turret.target != null &&
               (other.transform == turret.target || other.transform.IsChildOf(turret.target));
    }
}