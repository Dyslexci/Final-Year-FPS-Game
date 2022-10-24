using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperProjectile : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider col;

    float damage = 0;
    float speed = 0;
    float range = 1000;
    bool initialised;

    Vector3 startPos;

    private void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        col = GetComponent<SphereCollider>();
    }
    
    public void Initialise(float _speed, float _damage, float _range)
    {
        damage = _damage;
        speed = _speed;
        range = _range;
        startPos = transform.position;
        initialised = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialised) return;
        if (Vector3.Distance(transform.position, startPos) > range) Destroy(this);
        transform.Translate(transform.forward * Time.deltaTime * speed);
        if(Physics.CheckSphere(transform.position, col.radius, ~LayerMask.GetMask("Player")))
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, col.radius);
            for(int i = 0; i < hits.Length; i++)
            {
                if(hits[i].tag == "Enemy")
                {
                    hits[i].gameObject.SendMessage("DamageHealth", damage, SendMessageOptions.DontRequireReceiver);
                    Destroy(gameObject);
                }
            }
            Destroy(gameObject);
        }
    }

}
