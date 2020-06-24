using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playercontroller : MonoBehaviour
{
	
	private Rigidbody2D rb;
	private SpriteRenderer sp;
	private Animator ani;
	private float canJump = 0;
	public int[] inventory;
	public int inventoryAmount = 0;
	public GameObject inventoryUI;
	public GameObject[] inventorySlot;
	public Sprite[] itemSprites;
	public GameObject itemUI;
	public GameObject itemMenu;
	private int lastClickedItem = 0;
	
	//stats
	public float strenght = 1f, health = 10f;
	public float walkingSpeed, walkingAccel, runningSpeed, runningAccel, rollSpeed, sneakingSpeed, breakingSpeed, jumpingHeight;
    // Start is called before the first frame update
    void Start()
    {
		inventory = new int[inventorySlot.Length];
        rb = GetComponent<Rigidbody2D>();
		sp = GetComponent<SpriteRenderer>();
		ani = GetComponent<Animator>();
		//itemMenu.enabled = false;
		itemMenu.SetActive(false);
    }
    
    float jumpTimer = 0;
	float attackTimer = 0;
	float rollTimer = 0;

    // Update is called once per frame
    void Update()
    {
		inventoryUI.SetActive(Input.GetKey("e"));
		if(Input.GetKey("e")) {Time.timeScale = 0; return;}
		else Time.timeScale = 1;
        bool isRunning = Input.GetKey("left shift");
		bool sneaking = Input.GetKey(KeyCode.LeftControl);

		if(isRunning&&sneaking) isRunning=false;

		bool isMoving = (Mathf.Abs(rb.velocity.x) > 0.2f);
		bool isInAir = (Mathf.Abs(rb.velocity.y) > 0.1f);

		attackTimer -= Time.deltaTime;
		if(attackTimer > 0) return;


		rollTimer -= Time.deltaTime;
		if(rollTimer > 0) {
			rb.velocity = new Vector2( rollSpeed * (sp.flipX ? -1 : 1), rb.velocity.y);
			transform.GetChild(0).localScale = new Vector3(1, 0.5f, 1);
			return;
		}else if(!sneaking) transform.GetChild(0).localScale = new Vector3(1, 1, 1);

		if(rollTimer < 0 && attackTimer < 0) ani.SetInteger("state", 0);

		if(isInAir) {ani.SetInteger("state", 2); Debug.Log("jump");}
		else if(Input.GetButtonDown("Fire1")) {ani.SetInteger("state", 3); attackTimer = 1.5f;}
		else if(Input.GetButtonDown("Fire2")) {ani.SetInteger("state", 4); rollTimer = 1f;}
		else if(sneaking) ani.SetInteger("state", 5);
		else if(isMoving && !isInAir) ani.SetInteger("state", 1);
		else ani.SetInteger("state", 0);

		ani.SetFloat("y_Velocity", rb.velocity.y);

		if(sneaking) transform.GetChild(0).localScale = new Vector3(1, 0.5f, 1);
		if(Input.GetKeyDown(KeyCode.LeftControl)) transform.position -= new Vector3(0, 0, 1.25f);
		if(Input.GetKeyUp(KeyCode.LeftControl)) transform.position += new Vector3(0, 0, 1.25f);


		if(Input.GetAxis("Horizontal") != 0){ //movement
			sp.flipX = (Input.GetAxis("Horizontal") < 0);
			if(isRunning) rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, runningSpeed * Input.GetAxis("Horizontal"), runningAccel * Time.deltaTime),rb.velocity.y);
				else if(sneaking) rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, Input.GetAxis("Horizontal") * sneakingSpeed, walkingAccel * Time.deltaTime), rb.velocity.y);
				else rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, Input.GetAxis("Horizontal") * walkingSpeed, walkingAccel * Time.deltaTime), rb.velocity.y);
		}else //slowing Down
		{
			rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, Time.deltaTime * breakingSpeed), rb.velocity.y);
		}

		jumpTimer -= Time.deltaTime;
		canJump -= Time.deltaTime;
		if(Input.GetButton("Jump")) jumpTimer = 0.2f;


		if(Input.GetButton("Jump") && isRunning && canJump > 0) {rb.velocity = new Vector2(rb.velocity.x, jumpingHeight * 1.2f); canJump = 0;}
		if(Input.GetButton("Jump") && canJump > 0) {rb.velocity = new Vector2(rb.velocity.x, jumpingHeight); canJump = 0;}
		else if(!Input.GetButton("Jump") && rb.velocity.y > 0) rb.velocity /= 1 + Time.deltaTime * 3;
		if(rb.velocity.y < 0) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - Time.deltaTime * 9);


    }
	void UpdateInventory ()
	{
		for(int i = 0; i < inventoryAmount; i++)
		{
			if(inventory[i] == -1 && inventorySlot[i].transform.childCount == 1) Destroy(inventorySlot[i].transform.GetChild(0).gameObject);
			else if(inventory[i] == -1) continue;
			Debug.Log(inventorySlot[i].transform.childCount);
			if(inventorySlot[i].transform.childCount == 0)
			{
				GameObject tmp = Instantiate(itemUI, inventorySlot[i].transform.position, Quaternion.Euler(0,0,0));
				tmp.transform.parent = inventorySlot[i].transform;
				tmp.transform.localScale = inventoryUI.transform.localScale / 5f;
				tmp.GetComponent<Image>().sprite = itemSprites[inventory[i]]; 
			}
		
		
		}
	}
	
	void OnCollisionEnter2D(Collision2D col)
	{
		//add item to inventory
		if(col.gameObject.tag == "item")
		{
			if(inventoryAmount >= inventorySlot.Length) return;
			
			inventory[inventoryAmount] = col.gameObject.GetComponent<item>().type;
			Destroy(col.gameObject);
			inventoryAmount++;
			UpdateInventory();
		}
	}
	
	public void ItemMenuAppear(int item)
	{
		itemMenu.transform.position = Input.mousePosition;
		itemMenu.SetActive(true);
		lastClickedItem = item;
	}
	
	public void ItemUse ()
	{
		switch(lastClickedItem)
		{
			case 0: //healing
				health += 1;
				break;
			case 1: //speed
				walkingSpeed *= 1.1f;
				runningSpeed *= 1.1f;
				break;
			case 2: //strength
				strenght *= 1.1f;
				break;
		}
		inventoryDelete(lastClickedItem);
		
	}
	
	void inventoryDelete (int item)
	{
		inventory[item] = -1;
		UpdateInventory();
	}
    
    
    void OnTriggerStay2D(Collider2D other)
    {
	    canJump = 0.2f;
    }
    

}
