using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float lookAhead;
    public float moveTresholdX;
    public float moveTresholdY;
    public float yOffset;
    public Character Character;
    public float stiffness;
    public float focalStiffness;
    public float restartTime;

    private Material _mat;

    public enum Mode
    {
        normal,
        dashing
    }

    private Camera cam
    {
        get { return Camera.main; }
    }

    private void Start()
    {
        _mat = new Material(Shader.Find("Debug/Vertex Color"));
        _mat.color = Color.white;
        goalFocal = Vector2.one * .5f;
        focus = Character.transform.position;
    }

    Vector2 goalFocal;
    private Vector2 focalPoint;
    private Vector2 focus;

    private void FixedUpdate()
    {
        moveX();
        moveY();
        updateTimer(Time.fixedDeltaTime);
    }

    bool followX;
    bool followY;
    void moveY()
    {
        //if (Mathf.Abs(.5f - cam.WorldToViewportPoint(Character.transform.position).y) > moveTresholdY)
        //{
        //    followY = true;
        //}
        //if(Character.isGrounded)
        //{
        //    followY = false;
        //}
        //if (followY || Character.isGrounded)
        //{
        //    cam.transform.position = new Vector3(cam.transform.position.x, Mathf.Lerp(cam.transform.position.y, Character.transform.position.y, stiffness * Time.fixedDeltaTime), cam.transform.position.z);
        //}

        if (!followY)
        {
            if (Mathf.Abs(cam.WorldToViewportPoint(focus).y - cam.WorldToViewportPoint(Character.transform.position).y) > moveTresholdY)
            {
                followY = true;
            }
           
        }
        else
        {
            if (Character.isGrounded)
            {
                followY = false;
            }
            focus.y = Character.transform.position.y;
        }
        if(Character.isGrounded)
        {
            focus.y = Character.transform.position.y + cam.ViewportToWorldPoint(new Vector2(.5f, .5f + yOffset)).y - cam.transform.position.y;
        }

        goalFocal.y = .5f - (followY || Character.isGrounded ? Character.velocity.y*lookAhead : 0f);
        focalPoint.y = Mathf.Lerp(focalPoint.y, goalFocal.y, focalStiffness * Time.fixedDeltaTime);
        float focalCamDiff = cam.transform.position.y - cam.ViewportToWorldPoint(focalPoint).y;
        float camY = Mathf.Lerp(cam.transform.position.y, focus.y + focalCamDiff, stiffness * Time.fixedDeltaTime);

        cam.transform.position = new Vector3(transform.position.x, camY, transform.position.z);



    }

    void moveX()
    {
        if(!followX)
        {
            if( Mathf.Abs(cam.WorldToViewportPoint(focus).x - cam.WorldToViewportPoint(Character.transform.position).x) > moveTresholdX)
            {
                followX = true;
                resetLookAhead();
            } 
        }
        else
        {
            if(Mathf.Approximately(Character.velocity.x, 0f) && timer < 0f)
            {
                followX = false;
            }
            focus.x = Character.transform.position.x;
        }

        goalFocal.x = .5f - getLookAhead(followX ? Character.velocity.x : 0f);
        focalPoint.x = Mathf.Lerp(focalPoint.x, goalFocal.x, focalStiffness * Time.fixedDeltaTime);
        float focalCamDiff = cam.transform.position.x - cam.ViewportToWorldPoint(focalPoint).x;
        float camX = Mathf.Lerp(cam.transform.position.x, focus.x + focalCamDiff, stiffness * Time.fixedDeltaTime);

        cam.transform.position = new Vector3(camX, transform.position.y, transform.position.z);


    }

    float maxVel;
    float timer;
    float getLookAhead(float vel)
    {

        Move move = Character.GetComponent<Move>();

        vel = Mathf.Clamp(vel, -move.settings.speed, move.settings.speed);

        if (timer <= 0f || Mathf.Abs(maxVel) - Mathf.Abs(vel) < .1f || 
            (Mathf.Sign(maxVel) != Mathf.Sign(vel) && Mathf.Abs(vel) > .1f))
        {
            maxVel = vel;
            timer = restartTime;
        }

        return lookAhead * maxVel;
    }

    void resetLookAhead()
    {
        timer = 0f;
    }

    void updateTimer(float dt)
    {
        if(timer > 0f)
        {
            timer -= dt;
        }
    }

    


    public bool DEBUG = true;

    private void OnPostRender()
    {
        if (DEBUG)
        {
            drawDebugLines();
        }
    }

    void drawPos(float x, float y)
    {
        GL.Vertex3(x, y, 0f);
    }

    void drawDebugLines()
    {
        _mat.SetPass(0);
        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.magenta);
        drawPos(focalPoint.x - moveTresholdX, 1f);
        drawPos(focalPoint.x - moveTresholdX, -1f);
        drawPos(focalPoint.x + moveTresholdX, 1f);
        drawPos(focalPoint.x + moveTresholdX, -1f);

        drawPos(1f, focalPoint.y - moveTresholdY);
        drawPos(-1f, focalPoint.y - moveTresholdY);
        drawPos(1f, focalPoint.y + moveTresholdY);
        drawPos(-1f, focalPoint.y + moveTresholdY);

        GL.End();
        GL.PopMatrix();
    }
}