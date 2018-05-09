using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Experimental.PlayerLoop;

public class CameraController : MonoBehaviour
{
    public float lookAhead;
    public float moveTreshold;
    public Character Character;
    public float stiffness;
    public float focalStiffness;

    private Material _mat;

    private bool lerping;

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
    }

    Vector2 goalFocal;
    private Vector2 focalPoint;
    private Vector2 focus;

    private void FixedUpdate()
    {
        Vector2 playerPos = cam.WorldToViewportPoint(Character.transform.position);

        if (!lerping)
        {
            if (Mathf.Abs(playerPos.x - .5f) > moveTreshold)
            {
                lerping = true;
            }
        }
        else if (Character.velocity.sqrMagnitude < 0.1f)
        {
            lerping = false;
        }

        if (lerping)
        {
            focus.x = Character.transform.position.x;
        }

        goalFocal.x = .5f - (lerping ? lookAhead * Character.velocity.x : 0f);
        focalPoint.x = Mathf.Lerp(focalPoint.x, goalFocal.x, focalStiffness * Time.deltaTime);

        Vector3 camPos = cam.transform.position;
        Vector2 focalWorld = cam.ViewportToWorldPoint(focalPoint);
        camPos -= (Vector3) focalWorld;
        focalWorld.x = Mathf.Lerp(focalWorld.x, focus.x, stiffness * Time.deltaTime);

        cam.transform.position = camPos + (Vector3) focalWorld;
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
        drawPos(focalPoint.x - moveTreshold, 1f);
        drawPos(focalPoint.x - moveTreshold, -1f);
        drawPos(focalPoint.x + moveTreshold, 1f);
        drawPos(focalPoint.x + moveTreshold, -1f);
        GL.End();
        GL.PopMatrix();
    }
}