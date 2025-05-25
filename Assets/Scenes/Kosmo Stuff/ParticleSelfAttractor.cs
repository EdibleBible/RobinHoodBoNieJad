using UnityEngine;

public class ParticleSelfAttractor : MonoBehaviour
{
    public ParticleSystem ps;
    public int initialParticleCount = 100;
    public float attractionForce = 5f;   // siła przyciągania do środka
    public float orbitForce = 3f;        // siła orbitalna wokół emitera
    public float damping = 0.95f;        // tłumienie prędkości (0.9 - 0.99)
    public float maxSpeed = 5f;          // limit prędkości cząstek
    public float maxDistance = 3f;       // maksymalna odległość od centrum, po której reset prędkości

    private ParticleSystem.Particle[] particles;

    void Start()
    {
        if (ps == null)
        {
            Debug.LogError("Brak przypisanego ParticleSystem!");
            return;
        }

        // Wyłącz emisję w inspectorze, aby nie dodawać nowych cząstek ciągle

        // Emitujemy jednorazowo ustaloną liczbę cząstek
        ps.Emit(initialParticleCount);

        // Rezerwujemy tablicę na cząstki
        particles = new ParticleSystem.Particle[initialParticleCount];

        // Pobieramy emitowane cząstki
        ps.GetParticles(particles);

        // Ustawiamy cząstkom bardzo długie życie, aby nie znikały
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].startLifetime = 9999f;
            particles[i].remainingLifetime = 9999f;

            // Opcjonalnie możesz ustawić startową prędkość np. orbitalną, żeby od razu miały ruch
            Vector3 dir = (particles[i].position - transform.position).normalized;
            Vector3 tangent = Vector3.Cross(dir, Vector3.up).normalized;
            particles[i].velocity = tangent * orbitForce;
        }

        ps.SetParticles(particles, particles.Length);
    }

    void LateUpdate()
    {
        if (ps == null) return;

        int aliveParticles = ps.GetParticles(particles);

        Vector3 center = transform.position;

        for (int i = 0; i < aliveParticles; i++)
        {
            Vector3 toCenter = center - particles[i].position;
            Vector3 dirToCenter = toCenter.normalized;

            Vector3 attraction = dirToCenter * attractionForce;
            Vector3 orbit = Vector3.Cross(dirToCenter, Vector3.up).normalized * orbitForce;

            Vector3 force = attraction + orbit;

            // Tłumienie prędkości + dodanie siły
            particles[i].velocity = particles[i].velocity * damping + force * Time.deltaTime;

            // Limit prędkości
            if (particles[i].velocity.magnitude > maxSpeed)
                particles[i].velocity = particles[i].velocity.normalized * maxSpeed;

            // Resetowanie prędkości, jeśli cząstka zbyt daleko
            if (Vector3.Distance(particles[i].position, center) > maxDistance)
            {
                Vector3 dir = (particles[i].position - center).normalized;
                Vector3 tangent = Vector3.Cross(dir, Vector3.up).normalized;
                particles[i].velocity = tangent * orbitForce;
            }

            // Podtrzymujemy życie cząstki
            particles[i].remainingLifetime = 9999f;
        }

        ps.SetParticles(particles, aliveParticles);
    }
}