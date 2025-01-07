using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxController : MonoBehaviour
{

    public GameObject coinPrefab;
    public int coinAmount;

    public AudioSource audio;
    public AudioClip audioClip;

    IEnumerator Start()
    {

        yield return new WaitForSeconds(15f);

        for ( int i = 0; i  < coinAmount; i++)
        {
            CreateCoin();
            yield return new WaitForSeconds(0.5f);
        }

    }
        private void CreateCoin()
        {
            GameObject newCoin = Instantiate(coinPrefab, this.transform.position, Quaternion.identity);
            Rigidbody coinRb = newCoin.GetComponent<Rigidbody>();
            float randomX = Random.Range(-3f, 3f);
            float randomZ = Random.Range(-3f, 3f);

        //동전이 나올때마다 오디오가 나오게 하겠다.
        audio.PlayOneShot(audioClip);

            coinRb.AddForce(new Vector3(randomX, 6f, randomZ), ForceMode.Impulse);
            coinRb.AddTorque(new Vector3(randomX, randomX, randomX), ForceMode.Impulse);

        }
}
