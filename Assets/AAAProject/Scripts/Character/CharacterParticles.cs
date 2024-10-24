 using UnityEngine;

 public class CharacterParticles : MonoBehaviour
 {
     [SerializeField] private ParticleSystem IdleParticle;
     [SerializeField] private ParticleSystem HitParticle;


     public void PlayIdleParticle()
     {
         if (IdleParticle)
         {
             IdleParticle.gameObject.SetActive(true);
         }
     }

     public void StopIdleParticle()
     {
         if (IdleParticle)
         {
             IdleParticle.gameObject.SetActive(false);
         }
     }

     public void PlayHitParticle(Vector3 position)
     {
         if (HitParticle)
         {
             HitParticle.transform.position = position;
             HitParticle.Play();
         }
     }
 }
