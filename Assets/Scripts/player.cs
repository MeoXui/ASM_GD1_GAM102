﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    public Transform self, hpTransform;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rigidbody2d;
    public Animator animator;
    public AudioSource audioSource;

    public Slider hp;
    public Image fillImage;
    public GameObject bullet;

    public float jumpForce, moveSpeed;
    float startMS;
    public int jumpsNumber;
    int jumpCounter, timeCounter;

    public LayerMask lmEnemy;

    public List<AudioClip> audioClips;

    bool isOnAir;

    public gameManager gm;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            gm.AddScore(1);
            audioSource.PlayOneShot(audioClips[0]);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Apple")
        {
            takeDmg(-0.21f);
        }

        if(other.gameObject.tag == "Bullet")
        {
            takeDmg(0.21f);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Bomd")
        {
            takeDmg(1.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isOnAir = false;
            jumpCounter = jumpsNumber;
        }

        if (other.gameObject.tag == "Boar")
        {
            takeDmg(0.21f);
        }

        if (other.gameObject.tag == "Plant")
        {
            takeDmg(0.21f);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isOnAir = true;
            jumpCounter = jumpsNumber - 1;
        }

        if (other.gameObject.tag == "Bomb")
        {
            takeDmg(1.01f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        startMS = moveSpeed;
        timeCounter = 50;
        hp.maxValue = 1;
        hp.value = hp.maxValue;
        isOnAir = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.UpArrow)
            || Input.GetKeyDown(KeyCode.W)) jump();

        if (Input.GetKey(KeyCode.DownArrow)
            || Input.GetKey(KeyCode.S)) fastFall();

        if (gm.AdminPower)
        {
            if (Input.GetKey(KeyCode.LeftArrow)
                || Input.GetKey(KeyCode.A)) run(-1);
            else if (Input.GetKey(KeyCode.RightArrow)
                || Input.GetKey(KeyCode.D)) run(1);
            else idle();
        }
        else if (Input.GetKey(KeyCode.Q)) idle();
        else run(1);

        AnimProcess();
    }

    // FixedUpdate is called once per 0.02s
    void FixedUpdate()
    {
        moveSpeed = startMS + (self.position.x / 100);

        if (Input.GetKeyDown(KeyCode.P))
            if (gm.AddScore(-1))
                shot();

        if (gm.AdminPower && Input.GetKey(KeyCode.T))
        {
            float scale = self.localScale.x;
            Vector3 pos = self.position + new Vector3(0.59375f * scale, 1.0625f, 0),
                end = Vector3.right * scale;
            RaycastHit2D seeEnemy = Physics2D.Raycast(pos, end, 5f, lmEnemy);
            if (seeEnemy)
            {
                Debug.DrawLine(pos, pos + end * seeEnemy.distance, Color.red);
                Destroy(seeEnemy.transform.gameObject);
            }
            else Debug.DrawLine(pos, pos + end * 5, Color.green);
        }

        if (self.position.x < -15
            || self.position.y < -15
            || hp.value == 0)
            dead();
    }

    void AnimProcess()
    {
        if (rigidbody2d.velocity.x != 0)
        {
            animator.SetBool("idle", false);
            animator.SetBool("run", true);
        } else
        {
            animator.SetBool("idle", true);
            animator.SetBool("run", false);
        }

        if (rigidbody2d.velocity.y < 0 && isOnAir)
        {
            animator.SetBool("jump", false);
            animator.SetBool("fall", true);
        }
        else if (rigidbody2d.velocity.y > 0 && isOnAir)
        {
            animator.SetBool("jump", true);
            animator.SetBool("fall", false);
        }
        else
        {
            animator.SetBool("jump", false);
            animator.SetBool("fall", false);
        }
    }

    void idle()
    {
        rigidbody2d.velocity = new Vector2(0, rigidbody2d.velocity.y);
    }

    void run(int scale)
    {
        self.localScale = new Vector3(scale, 1, 1);
        hpTransform.localScale = new Vector3(scale, 1, 1);
        rigidbody2d.velocity = new Vector2(moveSpeed * scale, rigidbody2d.velocity.y);
    }

    void jump()
    {
        if (jumpCounter > 0)
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpForce);
            audioSource.PlayOneShot(audioClips[2]);
            jumpCounter--;
        }
    }

    void fastFall()
    {
        if (isOnAir)
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, rigidbody2d.velocity.y - 0.5f);
        }
    }

    void shot()
    {
        Vector3 pos = self.position + new Vector3(0.59375f, 1.0625f, 0);
        Instantiate(bullet, pos, Quaternion.identity);
    }

    void takeDmg(float dmg)
    {
        hp.value -= dmg;
        gm.HPColorChanging();
    }

    void dead()
    {
        audioSource.PlayOneShot(audioClips[1]);
        gm.dead();
    }
}
