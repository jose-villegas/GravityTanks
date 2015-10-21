using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float BulletLifeTime = 2f;
    public GameObject BulletObject;
    public float BulletSpeed = 5f;

    // Update is called once per frame
    private void Update()
    {
        if (!Input.GetButtonUp("Fire1")) return;

        var newProjectile = Instantiate(BulletObject, transform.position, transform.rotation) as GameObject;
        if (newProjectile == null) return;

        var pRigidbody = newProjectile.GetComponent<Rigidbody>();

        if (pRigidbody != null)
        {
            // tag the projectile as a coming from the player
            newProjectile.tag = "Player";
            // move towards forward vec3
            pRigidbody.velocity = BulletSpeed*transform.forward;
        }

        Destroy(newProjectile, BulletLifeTime);
    }
}