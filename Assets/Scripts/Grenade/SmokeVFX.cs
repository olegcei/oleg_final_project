using UnityEngine;
using UnityEngine.VFX;

public class SmokeController : MonoBehaviour
{
    public VisualEffect vfx;
    public float playRate = 1f;
    public float fadeStartTime = 12f;

    private float timer;
    private bool fading;

    public void PlaySmoke()
    {
        vfx.playRate = playRate;
        Debug.Log("Setting play rate to: {vfx.playRate}");
        vfx.Play();
        timer = 0f;
        fading = false;
    }

    void Update()
    {
        if (!vfx.aliveParticleCount.Equals(0) && !fading)
        {
            timer += Time.deltaTime;
            if (timer >= fadeStartTime)
            {
                vfx.Stop(); // stops spawning, existing particles age out per their own curve
                fading = true;
            }
        }
    }
}