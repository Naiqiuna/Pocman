using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip Clip1;
    public AudioClip Clip2;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(playSound());
    }
    IEnumerator playSound()
    {
        GetComponent<AudioSource>().clip = Clip1;
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(Clip1.length);
        GetComponent<AudioSource>().clip = Clip2;
        GetComponent<AudioSource>().Play();
        GetComponent<AudioSource>().loop = true;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
