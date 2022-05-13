using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Entities;
using SkillBridge.Message;
using Services;
using Managers;

public class PlayerInputController : MonoBehaviour {

    public Rigidbody rb;

    CharacterState state;

    public Character character;

    public float rotateSpeed = 2.0f;

    public float turnAngle = 10;

    public int speed;

    public EntityController entityController;

    public bool onAir = false;

    private NavMeshAgent agent;

    private bool autoNav = false;
    
    void Start ()
    {
        state = CharacterState.Idle;
        if(this.character == null)
        {
            DataManager.Instance.Load();
            NCharacterInfo cinfo = new NCharacterInfo();
            cinfo.Id = 1;
            cinfo.Name = "Test";
            cinfo.ConfigId = 1;
            cinfo.Entity = new NEntity();
            cinfo.Entity.Position = new NVector3();
            cinfo.Entity.Direction = new NVector3();
            cinfo.Entity.Direction.X = 0;
            cinfo.Entity.Direction.Y = 100;
            cinfo.Entity.Direction.Z = 0;
            this.character = new Character(cinfo);

            if (entityController != null)
            {
                entityController.entity = this.character;
            }
        }
        if (agent == null)
        {
            agent = this.gameObject.AddComponent<NavMeshAgent>();

            agent.stoppingDistance = 0.3f;
        }
    }

    /// <summary>
    /// 开始寻路
    /// </summary>
    /// <param name="target"></param>
    public void StartNav(Vector3 target)
    {
        StartCoroutine(BeginNav(target));
    }

    IEnumerator BeginNav(Vector3 target)
    {
        agent.SetDestination(target);
        yield return null;
        autoNav = true;
        if (state !=  CharacterState.Move)
        {
            state = CharacterState.Move;
            this.character.MoveForward();
            this.SendEntityEvent(EntityEvent.MoveFwd);
            agent.speed = this.character.speed / 100f;
        }
    }

    /// <summary>
    /// 停止寻路
    /// </summary>
    public void StopNav()
    {
        autoNav = false;
        agent.ResetPath();
        if (state != CharacterState.Idle)
        {
            state = CharacterState.Idle;
            this.rb.velocity = Vector3.zero;
            this.character.Stop();
            this.SendEntityEvent(EntityEvent.Idle);
        }
        NavPathRenderer.Instance.SetPath(null, Vector3.zero);
    }

    public void NavMove()
    {
        //寻路还没有完成
        if (agent.pathPending)
        {
            return;
        }

        //寻路失败了
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            StopNav();
            return;
        }

        //寻路还没有完成
        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            return;
        }
        //玩家自己点击了前进或者后退
        if (Mathf.Abs(Input.GetAxis("Vertical"))> 0.1 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1)
        {
            StopNav();
            return;
        }

        //已经快到了
        NavPathRenderer.Instance.SetPath(agent.path, agent.destination);
        if (agent.isStopped || agent.remainingDistance < 2f)
        {
            StopNav();
            return;
        }
    }

    void FixedUpdate()
    {
        if (character == null)
            return;

        if (autoNav)
        {
            NavMove();
            return;
        }

        if (InputManager.Instance != null && InputManager.Instance.IsInputMode)
        {
            return;
        }

        float v = 0f;
        if (SceneManager.Instance.NowLevelName() != "CharSelect")
        {
             v = Input.GetAxis("Vertical");
        }
        if (v > 0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            { 
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveForward();
                this.SendEntityEvent(EntityEvent.MoveFwd);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else if (v < -0.01)
        {
            if (state != SkillBridge.Message.CharacterState.Move)
            {
                state = SkillBridge.Message.CharacterState.Move;
                this.character.MoveBack();
                this.SendEntityEvent(EntityEvent.MoveBack);
            }
            this.rb.velocity = this.rb.velocity.y * Vector3.up + GameObjectTool.LogicToWorld(character.direction) * (this.character.speed + 9.81f) / 100f;
        }
        else
        {
            if (state != SkillBridge.Message.CharacterState.Idle)
            {
                state = SkillBridge.Message.CharacterState.Idle;
                this.rb.velocity = Vector3.zero;
                this.character.Stop();
                this.SendEntityEvent(EntityEvent.Idle);
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            this.SendEntityEvent(EntityEvent.Jump);
        }

        float h = Input.GetAxis("Horizontal");
        if (h<-0.1 || h>0.1)
        {
            this.transform.Rotate(0, h * rotateSpeed, 0);
            Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
            Quaternion rot = new Quaternion();
            rot.SetFromToRotation(dir, this.transform.forward);
            
            if(rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
            {
                character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
                rb.transform.forward = this.transform.forward;
                this.SendEntityEvent(EntityEvent.None);
            }

        }
        //Debug.LogFormat("velocity {0}", this.rb.velocity.magnitude);
    }

    Vector3 lastPos;
    float lastSync = 0;
    private void LateUpdate()
    {
        if (this.character == null) return;
        Vector3 offset = this.rb.transform.position - lastPos;
        this.speed = (int)(offset.magnitude * 100f / Time.deltaTime);
        //Debug.LogFormat("LateUpdate velocity {0} : {1}", this.rb.velocity.magnitude, this.speed);
        this.lastPos = this.rb.transform.position;

        //角色方向/位置改变过大时，向服务器发包请求同步
        if ((GameObjectTool.WorldToLogic(this.rb.transform.position) - this.character.position).magnitude > 50)
        {
            this.character.SetPosition(GameObjectTool.WorldToLogic(this.rb.transform.position));
            this.SendEntityEvent(EntityEvent.None);
        }
        this.transform.position = this.rb.transform.position;

        Vector3 dir = GameObjectTool.LogicToWorld(character.direction);
        Quaternion rot = new Quaternion();
        rot.SetFromToRotation(dir, this.transform.forward);

        if (rot.eulerAngles.y > this.turnAngle && rot.eulerAngles.y < (360 - this.turnAngle))
        {
            character.SetDirection(GameObjectTool.WorldToLogic(this.transform.forward));
            this.SendEntityEvent(EntityEvent.None);
        }
    }

    /// <summary>
    /// 客户端发送当前实体状态信息
    /// </summary>
    /// <param name="entityEvent"></param>
    public void SendEntityEvent(EntityEvent entityEvent, int param = 0)
    {
        if (entityController != null)
        {
            entityController.OnEntityEvent(entityEvent,param);
        }
        MapService.Instance.SendMapEntitySync(entityEvent, this.character.EntityData,param);

    }
}
