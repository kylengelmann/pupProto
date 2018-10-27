using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour {

    //public float translationStiffness = 30f;
    //public float impulse = 100f;
    //public float rotationStiffness = 5f;
    //public float rotationDrag = .5f;
    public float translationPower = .1f;
    public float rotaionPower = 8f;
    public float decay = .3f;
    public float sleepThreshold = .1f;
    float power;

    bool isSleeping;
    Vector2 pos;
    float rot;
    //Vector2 velocity;
    //float angularVelocity;

	void Start () {
	    //StartCoroutine(repeatShake(new WaitForSeconds(3)));
        GetComponent<CameraController>().Character.events.character.onHardLand += shake;
	}

    //IEnumerator repeatShake(WaitForSeconds wait)
    //{
    //    while(true) {
    //        shake(0, impulse);
    //        yield return wait;
    //    }
    //}
	
	void Update () {
        if(isSleeping) return;
        pos = Random.insideUnitCircle*translationPower*power;
        rot = (Random.value*2f - 1f)*rotaionPower*power;
        power = Mathf.Lerp(power, 0f, decay);
        if(power < sleepThreshold)
        {
            isSleeping = true;
        }
		//float angle = transform.rotation.eulerAngles.z;
  //      if(angle > 180f)
  //      {
  //          angle = angle - 360f;
  //      }
  //      if( Mathf.Abs(angle) > 0.5f || Mathf.Abs(angularVelocity) > 1f) {
  //          angularVelocity -= angle * rotationStiffness + angularVelocity*rotationDrag;
  //          angle += angularVelocity * Time.deltaTime;
  //      }
  //      else
  //      {
  //          angle = 0f;
  //          angularVelocity = 0f;
  //          isSleeping = true;
  //      }
  //      transform.rotation = Quaternion.Euler(0, 0, angle);
	}

    Vector3 lastPos;
    private void OnPreRender()
    {
        lastPos = transform.position;
        transform.SetPositionAndRotation(lastPos + (Vector3)pos, Quaternion.Euler(0, 0, rot));
    }

    private void OnPostRender()
    {
        transform.SetPositionAndRotation(lastPos, Quaternion.identity);
    }

    void shake()
    {
        isSleeping = false;
        //angularVelocity += impulse;
        power = 1f;

    }
}
