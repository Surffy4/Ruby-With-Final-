using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;

    public int maxHealth = 5;
    public int maxScore = 4;
    
    public GameObject projectilePrefab;
    public GameObject hitParticlesPrefab;

    public AudioClip throwSound;
    public AudioClip hitSound;

    int currentHealth;
    public int health { get { return currentHealth; }}
    public int score;
    public TextMeshProUGUI scoreText;
    public GameObject winTextObject;
    public GameObject loseTextObject;
    public AudioClip winsound;
    public AudioClip losesound;
    private bool playerWin;
    private bool playerLose;
    public GameObject bgsound;

    private bool level2;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    Rigidbody2D rigidbody2d;
    float horizontal; 
    float vertical;

    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        score = 0;
        SetScoreText();

        audioSource = GetComponent<AudioSource>();

        winTextObject.SetActive(false);
        loseTextObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
                        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
               NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        if (score == 4)
        {
            level2 = true;
        }
        
        //WINNING THE GAME
        if (score == 4)
        {
            speed = 0;
            playerWin = true;
            winTextObject.SetActive(true);
            Destroy(bgsound);
            PlaySound(winsound);
        }

        //LOSE CONDITION
        if (health == 0)
        {
            speed = 0;
            playerLose = true;
            PlaySound(losesound);
            loseTextObject.SetActive(true);
        }

        if(Input.GetKey(KeyCode.R) && playerLose == true)
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        //UPON A WIN
        if(Input.GetKey(KeyCode.R) && playerWin == true)
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlaySound(winsound);
        }
        //Lose Sound
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlaySound(losesound);
        }

    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + 3.0f * horizontal * Time.deltaTime;
        position.y = position.y + 3.0f * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;

            PlaySound(hitSound);
            GameObject hitParticleObject = Instantiate(hitParticlesPrefab, transform.position, Quaternion.identity);
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    public void ChangeScore()
    {
        score = score + 1;
        SetScoreText();
        if (score >= 4) 
		{
            winTextObject.SetActive(true);
            PlaySound(winsound);
            Destroy(bgsound);
		}
    }

    void SetScoreText()
	{
		scoreText.text = "Fixed Robots: " + score.ToString();
	}

    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");

        PlaySound(throwSound);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
