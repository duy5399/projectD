using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator anim;
    [SerializeField] private AudioSource audioSource;

    [Header("Movement")]
    [SerializeField] private bool canMove;
    [SerializeField] private Vector3 movement;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dazedTime;

    [Header("Jump")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool doubleJump;

    [Header("Dash")]
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private bool canDash;
    [SerializeField] private bool isDashing;
    [SerializeField] private float dashPower;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    void Awake()
    {
        photonView.Owner.TagObject = gameObject;
        this.rb2d = GetComponent<Rigidbody2D>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.anim = GetComponent<Animator>();
        this.audioSource = GetComponent<AudioSource>();
        this.groundCheckPoint = this.transform.GetChild(0);
        tr = GetComponent<TrailRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        moveSpeed = 6f;
        jumpForce = 25f;

        canDash = true;
        isDashing = false;
        dashPower = 10f;
        dashTime = 0.4f;
        dashCooldown = 3f;

        //PhotonNetwork.SendRate = 20;          //Xác định số lần PhotonNetwork gửi đi 1 gói dữ liệu- càng cao càng ít độ trễ cho game nhưng tốn tài nguyên
        //PhotonNetwork.SerializationRate = 15; //xác định số lần hàm OnPhotonSerialize được gọi - càng cao càng ít độ trễ cho game nhưng tốn tài nguyên
        if (photonView.IsMine)  //PhotonView: đối tượng trên mạng (xác định bằng viewID)
        {
            txtDisplayname.text = PhotonNetwork.NickName;     //hiện tên Player của mình
            txtDisplayname.color = Color.yellow;

            joystick = FindObjectOfType<Joystick>();    //tìm Fixed Joytick gán vào joystick

        }
        else
        {
            txtDisplayname.text = photonView.Owner.NickName;  //pv: 1 đối tượng trên mạng qua PhontonView - bất cứ ai là chủ sở hữu pv thì sẽ lấy nametext của pv đó
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (isDashing)
            {
                return;
            }
            if (canMove)
            {
                UpdateMovement();
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        UpdateAnimation();
    }

    protected virtual void UpdateMovement()
    {
        movement = new Vector3(joystick.Horizontal, 0);
        //movement.x = Input.GetAxisRaw("Horizontal");
        transform.position += movement * moveSpeed * Time.deltaTime;      //Time.delta là khoảng thời gian giữa 2 frame
        anim.SetFloat("moveSpeed", Mathf.Abs(movement.x));
    }

    public void UpdateJump()
    {
        if (GroundCheck() && canMove)
        {
            //Debug.Log("GroundCheck = true");
            this.rb2d.velocity = Vector2.up * jumpForce;
            doubleJump = true;
            AudioManager.instance.PlayerJumpSFX(audioSource);
        }
        else if (doubleJump)
        {
            //Debug.Log("GroundCheck = false");
            this.rb2d.velocity = Vector2.up * jumpForce * 0.7f;
            doubleJump = false;
            AudioManager.instance.PlayerJumpSFX(audioSource);
        }
    }

    protected bool GroundCheck()
    {
        return Physics2D.OverlapCapsule(groundCheckPoint.position, new Vector2(0.15f, 0.03f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    }

    [PunRPC]
    protected void RPC_Flip(float x)
    {
        if (x >= 0.1f)
        {
            spriteRenderer.flipX = false;
            Vector3 newPos = new Vector3(transform.position.x + 0.5f, transform.GetComponent<PlayerCombat>().attackPoint_.position.y, transform.GetComponent<PlayerCombat>().attackPoint_.position.z);
            transform.GetComponent<PlayerCombat>().OnChangeAttackPoint(newPos);

        }
        else if (x <= -0.1f)
        {
            spriteRenderer.flipX = true;
            Vector3 newPos = new Vector3(transform.position.x - 0.5f, transform.GetComponent<PlayerCombat>().attackPoint_.position.y, transform.GetComponent<PlayerCombat>().attackPoint_.position.z);
            transform.GetComponent<PlayerCombat>().OnChangeAttackPoint(newPos);
        }
    }

    protected void UpdateAnimation()
    {
        if (movement.x != 0)
        {
            photonView.RPC("RPC_Flip", RpcTarget.AllBuffered, movement.x);
            //bool x = movement.x > 0 ? transform.GetComponent<SpriteRenderer>().flipX = false : transform.GetComponent<SpriteRenderer>().flipX = true;
        }
        if (GroundCheck())
        {
            anim.SetBool("isGrounded", true);
        }
        else
        {
            anim.SetBool("isGrounded", false);
        }
        anim.SetFloat("velocity.y", this.rb2d.velocity.y);
    }

    public IEnumerator SlowEffect(float _moveSpeed,float _interval)
    {
        moveSpeed = _moveSpeed;
        yield return new WaitForSeconds(_interval);
        moveSpeed = 6f;
    }

    public void SetMoveSpeed(float _moveSpeed)
    {
        this.moveSpeed = _moveSpeed;
    }

    public void SetCanDash(bool _canDash)
    {
        this.canDash = _canDash;
    }

    public IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        anim.SetTrigger("dash");
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0f;
        int flipX = spriteRenderer.flipX == true ? -1 : 1;
        this.rb2d.velocity = new Vector2(flipX * dashPower, 0);
        tr.emitting = true;
        yield return new WaitForSeconds(dashTime);
        tr.emitting = false;
        rb2d.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void UpdateDash()
    {
        if (canDash)
        {
            StartCoroutine(Dash());
            AudioManager.instance.PlayerDashSFX(audioSource);
        }
    }

    private void SetLayer(string _layer)
    {
        gameObject.layer = LayerMask.NameToLayer(_layer);
    }

    //****************
    [SerializeField] private Joystick joystick;

    public TextMeshProUGUI txtDisplayname;

    public Vector3 smoothMove;     //sẽ dùng gán giá trị vị trí di chuyển của người chơi khác
    public Quaternion smoothMoveRotation;  //dùng để gán vị trí xoay mặt của người chơi khác

    public void SmoothMovement()   //hàm cập nhập vị trí của người chơi khác khi smoothMove và smoothMoveRotation  được update vị trí mới từ hàm OnPhotonSerializeView();
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 10);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)  //hàm dùng để gửi vị trí của mình khi di chuyển đến những người chơi khác và nhận vị trí của những người chơi khác khi họ di chuyển nhân vật
    {                                                                               //PhotonStream stream: dữ liệu chúng ta gửi
        if (stream.IsWriting)       //ta gửi (send) dữ liệu - vị trí đi là đang writing
        {
            stream.SendNext(transform.position);
            //stream.SendNext(currentHealth);
            //stream.SendNext(transform.rotation);
            //stream.SendNext(transform.localScale);
        }
        else if (stream.IsReading)  //ta nhận (Receive) dữ liệu - vị trí của người khác là đang reading
        {
            smoothMove = (Vector3)stream.ReceiveNext(); //cập nhập mỗi lần và hàm smoothMovement(); sẽ hiện vị trí Player Y (của Hoài điều khiển) lên PC1 của Duy (Duy điều khiển Player X)
            //smoothHeal = (float)stream.ReceiveNext();
            //this.transform.localScale = (Vector3)stream.ReceiveNext();
            //smoothMoveRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
