namespace DungeonDefence
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UI_Projectile : MonoBehaviour
    {
        private Renderer rend;

        public delegate void AttackCallback(long index, long target);
        public ParticleSystem ps;
        public Data.ProjectileID id = Data.ProjectileID.archerarrow;

        private Vector3 _target = Vector3.zero;
       // private UI_Player _Player = null;
        private Vector3 _targetPosition = Vector3.zero;
        private Vector3 _start = Vector3.zero;
        public bool active = false;
        private float time = 0;
        private float timer = 0;
        public float damage = 10;
        public bool canDamage = true;

        void Start()
        {
            rend = GetComponent<Renderer>();
            rend.enabled = true;
        }
    
        public void Initialize(Vector3 start, Vector3 target, float speed)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(start, target);
                time = distance / speed;
                timer = 0;
                _start = start;
                _target = target;
                _targetPosition = _target;
                active = true;
                //transform.position = _target.transform.position;
            }
        }

        private void Update()
        {
            if (active && timer < 10f)
            {
                timer += Time.deltaTime;
                if(timer > time) { timer = time; }
                if(_target != null)
                {

                 // _targetPosition = _target.transform.position;
                 
                    float distance = Vector3.Distance(_targetPosition, transform.position);
                    if(distance < 0.1)
                    {
                       // Destroy(gameObject);
                        ps.Play();
                        active = false;
                        rend.enabled = false;
                    }
                
                }
                
                transform.position = Vector3.Lerp(_start, _targetPosition, timer / time);
                
               
            }
            else
            {
                if(id == Data.ProjectileID.boulder)
                {
                    if(!ps.isEmitting)
                        Destroy(gameObject);
                }
                else
                    Destroy(gameObject);
            }
        }
         void OnCollisionEnter(Collision collision)
         {
             // PLayer damage callback?
             Debug.Log("projectile collided:" + collision.gameObject);
         }

         

    }
}