using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pixelflag.monster1
{
    public class BigRock : MassObject
    {
        [SerializeField]
        private GameObject smallRockPrefab;

        public float underLimit = -300;
        public int spawnNum = 3;
        public int scatterPower = 30;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.tag == "Player" || collider.tag == "Trrain")
            {
                for (int i = 0; i < spawnNum; i++)
                {
                    SmallRock rock = Instantiate(smallRockPrefab).GetComponent<SmallRock>();
                    rock.Initialize();
                    rock.position = position;

                    Vector2 force = GetPrevVelocity2D();
                    force.x = Random.Range(scatterPower, -scatterPower);
                    force.y = -force.y * 0.8f;

                    rock.AddForce(force);
                }
                Destroy(gameObject);
            }
        }

        public void Update()
        {
            AfterUpdate();

            if (transform.position.y < underLimit)
                Destroy(gameObject);
        }
    }
}