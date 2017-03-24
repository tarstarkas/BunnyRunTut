using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BunnyController : MonoBehaviour {

    private Rigidbody2D myRigidBody;
    private Animator myAnim;
    private Collider2D myCollider;
    public float bunnyJumpForce = 500f;
    private float bunnyHurtTime = -1;
    public Text scoreText;
    private float startTime;
    private int jumpsLeft = 1;
    public AudioSource jumpSFX;
    public AudioSource deathSFX;

    // Use this for initialization
    void Start () {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        myCollider = GetComponent<Collider2D>();
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }

        if (bunnyHurtTime == -1)
        {

          if ((Input.GetButtonUp("Jump") || Input.GetButtonUp("Fire1")) && jumpsLeft > 0)
          {
                if(myRigidBody.velocity.y < 0)
                {
                    myRigidBody.velocity = Vector2.zero;
                }
                if(jumpsLeft == 1)
                {
                    myRigidBody.AddForce(transform.up * bunnyJumpForce*0.75f);
                }
                else
                {
                    myRigidBody.AddForce(transform.up * bunnyJumpForce);
                }
                jumpsLeft--;
                jumpSFX.Play();
          }
          myAnim.SetFloat("vVelocity",    Mathf.Abs(myRigidBody.velocity.y));
            scoreText.text = (Time.time - startTime).ToString("0.0");
        }
        else
        {
            if(Time.time > bunnyHurtTime + 2)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            foreach (PrefabSpawner prefabSpawn in FindObjectsOfType<PrefabSpawner>())
            {
                prefabSpawn.enabled = false;
            }
            foreach (MoveLeft moveLefter in FindObjectsOfType<MoveLeft>()){
                moveLefter.enabled = false;
            }
            bunnyHurtTime = Time.time;
            myAnim.SetBool("bunnyHurt", true);
            myRigidBody.velocity = Vector2.zero;
            myRigidBody.AddForce(transform.up * bunnyJumpForce);
            myCollider.enabled = false;
            deathSFX.Play();
            float currentBestScore = PlayerPrefs.GetFloat("HighScore", 0);
            float currentScore = Time.time - startTime;
            if(currentScore > currentBestScore)
            {
                PlayerPrefs.SetFloat("HighScore", currentScore);
            }
        }
        else if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            jumpsLeft = 2;
        }
    }

}
