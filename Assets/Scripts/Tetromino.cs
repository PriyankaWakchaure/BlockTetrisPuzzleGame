using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;


public class Tetromino : MonoBehaviour
{
    private SpriteRenderer sprite;
    private GameObject Tetrominoes;

    private Vector2 offset;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private bool isMovable;
    private bool isDroping;
    private bool isValidMove;
    private static bool isGameover;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        Tetrominoes = GameObject.Find("Tetrominoes");

        initialPosition = transform.position;

        isGameover = false;
        isGameover = false;
    }

    private void FixedUpdate()
    {
        if (!isValidMove)
        {
            transform.position = Vector2.MoveTowards(transform.position, initialPosition, 10f * Time.deltaTime);
            sprite.sortingOrder = 4;
        }

        if (Vector2.Distance(transform.position, initialPosition) < 0.01 && !isValidMove)
        {
            isValidMove = true;
            sprite.sortingOrder = 3;
        }

        Drop();

        if (isGameover) WinAnimation();
    }

    private void OnMouseDown()
    {
        isMovable = false;
        sprite.sortingOrder = 4;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseDrag()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos + offset;
    }

    private void OnMouseUp()
    {
        sprite.sortingOrder = 3;
        SnapToValidPos();
        CheckValidMove();
        CheckWinCondition();
        isMovable = true;
    }

    void SnapToValidPos()
    {
        HashSet<float> x = new HashSet<float>();
        HashSet<float> y = new HashSet<float>();

        Vector2 pos;
        float total = 0;

        foreach (Transform child in transform)
        {
            x.Add(Mathf.Round(child.position.x));
            y.Add(Mathf.Round(child.position.y));
        }

        foreach (var item in x) total += item;

        pos.x = total / x.Count;
        total = 0;

        foreach (var item in y) total += item;

        pos.y = total / y.Count;

        transform.position = pos;
    }

    private void CheckValidMove()
    {
        if (transform.position.x > 6 || transform.position.x < 0
            || transform.position.y > 9 || transform.position.y < 0)
        {
            isValidMove = false;
            return;
        }

        foreach (Transform child in transform)
        {
            if (!child.GetComponent<SquareBox>().CheckValidPos()) isValidMove = false;
        }
    }

    private void Drop()
    {
        if (isMovable && !isGameover && !isDroping && isValidMove)
        {
            if (transform.position.x <= 6 && transform.position.x >= 0
                && transform.position.y <= 9 && transform.position.y >= 0)
            {
                isDroping = true;
                foreach (Transform child in transform)
                {
                    Collider2D[] colliders = Physics2D.OverlapCircleAll(child.position - new Vector3(0, 0.5f, 0), 0.4f);

                    foreach (var collider in colliders)
                    {
                        if (collider.gameObject != transform.gameObject)
                        {
                            if (collider.tag == "Tetromino" || collider.tag == "Boundary")
                            {
                                isDroping = false;
                            }
                        }
                    }
                }

                if (isDroping) targetPosition = transform.position - new Vector3(0, 1, 0);
            }
        }

        if (isDroping)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, 10f * Time.deltaTime);
        }

        if (Vector2.Distance(transform.position, targetPosition) == 0)
        {
            isDroping = false;
        }

        if (isMovable && !isGameover && !isDroping && isValidMove)
        {
            if (transform.position.x <= 6 && transform.position.x >= 0
                   && transform.position.y <= 9 && transform.position.y >= 0)
                SnapToValidPos();
        }
    }

    private void CheckWinCondition()
    {
        isGameover = true;

        foreach (Transform child in Tetrominoes.transform)
        {
            if (child.position.x > 6 || child.position.x < 0
                || child.position.y > 9 || child.position.y < 0 || !isValidMove)
            {
                isGameover = false;
            }
        }

        if (isGameover) Invoke(nameof(LoadScene), 10f);
    }

    private void WinAnimation()
    {
        sprite.sortingOrder = 1; 
        foreach (Transform child in transform)
        {
            if(child.position.y > 9.5) child.GetComponent<Light2D>().enabled = false;
            else child.GetComponent<Light2D>().enabled = true;
        }
        transform.position += new Vector3(0, 1f, 0) * Time.deltaTime;
        if (transform.position.y > 11.0f) gameObject.SetActive(false);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Gameover");
    }
}