using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pup_sounds : MonoBehaviour {

  public AudioSource dog;
  public AudioClip wooshClip;
  public AudioClip jumpClip;
  public AudioClip fallClip;

  Character character;

  public void StopClip()
  {
    dog.Stop();
  }

  public void Woosh()
  {
    dog.clip = wooshClip;
    dog.Play();
  }

  public void Jump()
  {
    dog.clip = jumpClip;
    dog.Play();
  }

  public void StartFall()
  {
    dog.clip = fallClip;
    dog.Play();
  }

  // Use this for initialization
  void Start () {

    character = GetComponent<Character>();
    character.events.jump.onJump += Jump;

	}
	
	// Update is called once per frame
	void Update () {

    if (character.isGrounded && dog.clip == fallClip || character.velocity.y >= 0f && dog.clip == fallClip)
    {
      dog.Stop();
    }

	}
}
