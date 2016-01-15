using UnityEngine;
using System.Collections;

/**
 * The Monster class is responsible for controlling a Monster GameObject.
 * It pathfinds to a target using a NavMeshAgent and attacks the player via raycasting.
 * It is also reponsible for playing the Monster's AudioClips and animation clips.
 */
[RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(AudioSource))]
public class Monster : MonoBehaviour {

	[SerializeField]
	private float Health = 50.0f;

	[SerializeField]
	private float AttackCooldown = 0.8f;

	[SerializeField]
	private float AttackRange = 3.0f;

	[SerializeField]
	private float Damage = 25.0f;

    [SerializeField]
    private float DeathTime = 3.0f;

    [SerializeField]
    private float KnockbackForce = 1000.0f;

    [SerializeField]
    private AudioClip DeathSound;

    [SerializeField]
    private AudioClip HitSound;

    [SerializeField]
    private AudioClip[] AttackSounds;

    [SerializeField]
    private AudioClip[] Moans;

	private Transform target;
    private NavMeshAgent agent;
	private float cooldownTimer;
    private float moanTimer;

    private Animator animator;
    private AudioSource audioSource;

    public bool Dead;
    private bool hurt;

	/**
	 * A manually-called constructor.
	 */
	public void Initialize() {
		target = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
		cooldownTimer = 0.0f;
        moanTimer = 0.0f;

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

	/**
	 * The Update method is called automatically by MonoBehaviours once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
	private void Update() {
        if (Dead == true) {
            agent.SetDestination(this.transform.position);
            return;
        }

        if (Health <= 0.0f) {
            StartCoroutine(Die());
            return;
        }

		if (!target.GetComponent<Player>().CurrentArea.Equals("Central Area") && !hurt) {
            Vector3 destination = target.position;
            destination.y = this.transform.position.y;
            agent.SetDestination(destination);
            attack();
            animator.SetFloat("speed", agent.velocity.magnitude);
            if (distanceFromTarget() <= AttackRange + 1.0f) {
                this.transform.LookAt(target.position);
            } else {
                this.transform.LookAt(this.transform.position + agent.velocity);
            }
            this.transform.rotation = Quaternion.Euler(0.0f, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
        } else {
            animator.SetFloat("speed", 0.0f);
            agent.SetDestination(this.transform.position);
        }

        moan();
    }

	/**
	 * Attacks the target in range if not on cooldown.
	 */
	private void attack() {
		if (cooldownTimer == 0.0f) {
			cooldownTimer = Time.time;
		}

		if (Time.time - cooldownTimer >= AttackCooldown) {
			if (distanceFromTarget() <= AttackRange) {
                animator.Play("monster2Attack1", -1);
				RaycastHit rayHit;
				Debug.DrawRay(this.transform.position, this.transform.forward * AttackRange, Color.red);
				if (Physics.Raycast(this.transform.position, this.transform.forward, out rayHit, AttackRange)) {
					
					if (rayHit.collider.gameObject.name.Contains("Player")) {
						PlayerController p = rayHit.collider.gameObject.GetComponent<PlayerController>();
						p.TakeDamage(Damage, this.transform.position, KnockbackForce, AttackRange);
					}
					
				}
				cooldownTimer = 0.0f;
                audioSource.PlayOneShot(AttackSounds[Random.Range(0, AttackSounds.Length)]);
            } else {
				cooldownTimer = Time.time;
            }
		}
	}

	/**
	 * Returns the distance from the target.
	 * 
	 * @return float value of the distance from the target
	 */
	private float distanceFromTarget() {
		return (this.transform.position - target.position).magnitude;
	}

	/**
	 * Periodically moans (plays an AudioClip) every few random seconds between 4 and 9.
	 */
    private void moan() {
        if (Time.time - moanTimer >= 0) {
            moanTimer = Time.time + Random.Range(4, 9);
            audioSource.PlayOneShot(Moans[Random.Range(0, Moans.Length)]);
        }
    }

	/**
	 * A coroutine method that makes the Monster die.
	 * 
	 * @return an IEnumerator used for the StartCoroutine method to wait for some specified number of seconds
	 */
    public IEnumerator Die() {
        Dead = true;
        animator.Play("monster2Die", -1);
        audioSource.PlayOneShot(DeathSound);
        yield return new WaitForSeconds(DeathTime);
        Destroy(this.gameObject);
    }

	/**
	 * A coroutine method for when the Monster takes damage.
	 * 
	 * @return an IEnumerator used for the StartCoroutine method to wait for some specified number of seconds
	 */
    public IEnumerator TakeDamage(float damage) {
        if (Dead) yield break;
        animator.Play("monster2Hit1", -1);
        audioSource.PlayOneShot(HitSound);
        Health -= damage;
        hurt = true;
        yield return new WaitForSeconds(1);
        hurt = false;
	}
}
