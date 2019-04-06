using UnityEngine;

// Press the space key in Play Mode to switch to the Bounce state.

public class Move : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (null != anim)
            {
                // play Bounce but start at a quarter of the way though
                anim.Play("Bounce", 0, 0.25f);
            }
        }
    }
}