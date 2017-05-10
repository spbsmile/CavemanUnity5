using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
    [SerializeField] private AudioSource soundSource;
    [SerializeField] private AudioClip hitMusic;


    void Start () {}

    public void Test()
    {
        print("hello test");
        soundSource.PlayOneShot(hitMusic);
    }

   


}
