using UnityEngine;

// Lleva la cuenta de la vida del personaje: quitar vida, curar, regenerar y morir.
public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public float maxHealth = 100f;      // Vida maxima del personaje.
    public bool destroyOnDeath = false; // Si esta marcado, el objeto se destruye al morir.

    [Header("Regeneracion")]
    public float regenDelay = 5f;       // Segundos sin recibir dano antes de empezar a regenerar.
    public float regenPerSecond = 10f;  // Vida que se recupera por segundo.

    [Header("Estado (solo lectura)")]
    public float currentHealth;         // Vida actual. Se ve en el Inspector para comprobar que funciona.
    public bool isDead;                 // Se pone a true cuando la vida llega a 0.

    [Header("UI de muerte")]
    [SerializeField] private GameObject deathTextPanel;    // Panel con el texto.
    [SerializeField] private GameObject blackScreenPanel;  // Panel de pantalla negra.

    [SerializeField] private BloodSplatterHealthUI bloodUI;

    private float lastDamageTime;       // Momento del ultimo dano recibido.

    // Awake se ejecuta al principio, antes del primer frame.
    private void Awake()
    {
        // El personaje empieza la partida con la vida al maximo.
        currentHealth = maxHealth;
        isDead = false;
        lastDamageTime = -regenDelay;

        // Los paneles de muerte empiezan apagados.
        if (deathTextPanel != null) deathTextPanel.SetActive(false);
        if (blackScreenPanel != null) blackScreenPanel.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;

        // Solo regeneramos si falta vida y ha pasado suficiente tiempo sin dano.
        if (currentHealth < maxHealth && Time.time - lastDamageTime >= regenDelay)
        {
            currentHealth += regenPerSecond * Time.deltaTime;

            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }

            // Se actualiza cada frame para que la opacidad baje de forma suave.
            if (bloodUI != null)
            {
                bloodUI.UpdateHealth(currentHealth, maxHealth);
            }
        }
    }

    // Quita vida al personaje. Lo llama el proyectil de la torreta al impactar.
    public void TakeDamage(float damage)
    {
        // Si ya esta muerto no hacemos nada mas.
        if (isDead)
        {
            return;
        }

        currentHealth = currentHealth - damage;
        lastDamageTime = Time.time; // Reinicia la cuenta atras de la regeneracion.

        // Evitamos que la vida baje de 0.
        if (currentHealth < 0f)
        {
            currentHealth = 0f;
        }

        if (bloodUI != null)
        {
            bloodUI.UpdateHealth(currentHealth, maxHealth);
        }

        // Si se ha quedado sin vida, muere.
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // Suma vida al personaje (botiquines, zonas de curacion, etc.).
    public void Heal(float amount)
    {
        // Un personaje muerto no se puede curar.
        if (isDead)
        {
            return;
        }

        currentHealth = currentHealth + amount;

        // Evitamos pasarnos de la vida maxima.
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (bloodUI != null)
        {
            bloodUI.UpdateHealth(currentHealth, maxHealth);
        }
    }

    // Se llama una sola vez, cuando la vida llega a 0.
    private void Die()
    {
        isDead = true;

        Debug.Log("Ha muerto.");

        // Encendemos los paneles de muerte.
        if (blackScreenPanel != null) blackScreenPanel.SetActive(true);
        if (deathTextPanel != null) deathTextPanel.SetActive(true);

        if (destroyOnDeath)
        {
            // Opcion 1: el objeto desaparece de la escena.
            Destroy(gameObject);
        }
        else
        {
            // Opcion 2: el objeto se queda, pero apagamos el script de movimiento
            // para que el jugador ya no pueda controlarlo.
            PlayerMovement movement = GetComponent<PlayerMovement>();

            if (movement != null)
            {
                movement.enabled = false;
            }
        }
    }
}