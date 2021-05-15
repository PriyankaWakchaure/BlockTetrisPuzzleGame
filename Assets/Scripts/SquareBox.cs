using UnityEngine;

public class SquareBox : MonoBehaviour
{
    public bool CheckValidPos()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        foreach (var collider in colliders)
        {
            if (collider.gameObject != transform.parent.gameObject)
            {
                if (collider.tag == "Tetromino" || collider.tag == "Boundary")
                {
                    return false;
                }
            }
        }
        return true;
    }
}
