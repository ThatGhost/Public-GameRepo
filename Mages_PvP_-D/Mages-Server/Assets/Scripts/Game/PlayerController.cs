using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using MagesServer.Enums;
using MagesServer.DataLayer;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private CapsuleCollider _coll;

    [Header("General")]
    [SerializeField] private int _PrecisionScale;
    [SerializeField] private int _Friction;
    [SerializeField] private int _Gravity;
    [SerializeField] private int _MaxDownwardsSpeed;
    [SerializeField] private Transform _Feet;
    [SerializeField] private float _FeetDistance;
    [SerializeField] private int _DirectionMultiplier;
    [SerializeField] private LayerMask _LayerMask;

    [Header("Mouse")]
    [SerializeField] private float _LookSpeed;
    [SerializeField] private float _LookLimit;

    [Header("Hover")]
    [SerializeField] private float _HoverHeight;

    [Header("Jumping")]
    [SerializeField] private int _JumpForce;

    [Header("Walking")]
    [SerializeField] private int _WalkingForce;
    [SerializeField] private int _WalkingMaxSpeed;
    [SerializeField] private int _WalkingAccel;
    [SerializeField] private int _WalkingDecel;

    [Header("Running")]
    [SerializeField] private int _RunningForce;
    [SerializeField] private int _RunningMaxSpeed;
    [SerializeField] private int _RunningAccel;
    [SerializeField] private int _RunningDecel;

    [Header("Crouching")]
    [SerializeField] private int _CrouchingFactor;

    [NonSerialized]public IntVector3D _velocity = IntVector3D.zero;
    [NonSerialized] public IntVector3D _position = IntVector3D.zero;
    [NonSerialized] public float _RotationX=0;
    public Transform _CamTran;
    
    private Deque<InputBits> _InputQueue = new Deque<InputBits>();
    public bool _Grounded = false;
    private bool _Jumping = false;
    public bool _Crouching = false;
    public bool _Running = false;
    private Vector3 _ScaleVector;

    private Dictionary<GameObject, ContactPoint[]> _Collisions = new Dictionary<GameObject, ContactPoint[]>();
    private List<KeyValuePair<IntVector3D, ForceMode>> _AdditiveForcesBuffer = new List<KeyValuePair<IntVector3D, ForceMode>>();

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<CapsuleCollider>();
        _ScaleVector = new Vector3(1f / _PrecisionScale, 1f / _PrecisionScale, 1f / _PrecisionScale);
        Vector3 currentpos = transform.position;
        _position = new IntVector3D((int)(currentpos.x * _PrecisionScale), (int)(currentpos.y * _PrecisionScale), (int)(currentpos.z * _PrecisionScale));
        GameServer.onTick += PhysicsUpdate;
    }

    private void OnDestroy()
    {
        GameServer.onTick -= PhysicsUpdate;
    }

    private void PhysicsUpdate(uint tick)
    {
        if (_InputQueue.Count > 0)
        {
            InputBits bits = _InputQueue.RemoveFromBack();
            Movement(bits);
            HandleMouse(bits.x, bits.y);
            NextPhysicsStep();
            ApplyPosition();
            //print($"tick {tick} :: position: {_position}, vel: {_velocity}");
        }

        if (_InputQueue.Count != 0)
            PhysicsUpdate(tick);
    }

    private void Movement(InputBits input)
    {
        //translate input
        Vector3 inputdir = new Vector3(((input.inputBits & (uint)InputType.Left)!= 0 ? -1 : 0) + ((input.inputBits & (uint)InputType.Right) != 0 ? 1 : 0)
                                        ,0,
                                      ((input.inputBits & (uint)InputType.Down) != 0 ? -1 : 0) + ((input.inputBits & (uint)InputType.Up) != 0 ? 1 : 0));
        inputdir = (transform.forward * inputdir.z) + (transform.right * inputdir.x);

        bool crouching = (input.inputBits & (uint)InputType.Crouching) != 0;
        _Running = (input.inputBits & (uint)InputType.Running) != 0;

        //crouching
        if(!crouching && _Crouching)
        {
            Ray ray = new Ray(transform.position, Vector3.up);
            if (Physics.Raycast(ray, 1.2f))
            {
                crouching = true;
            }
            else
            {
                _coll.height = 2;
                _coll.center += new Vector3(0,0.5f,0);
                _CamTran.position += new Vector3(0, 0.5f, 0);
            }
        }
        else if(crouching && !_Crouching)
        {
            _coll.height = 1;
            _coll.center += new Vector3(0, -0.5f, 0);
            _CamTran.position += new Vector3(0,-0.5f,0);
        }

        _Crouching = crouching;

        //making wish direction
        IntVector3D WishDir = new IntVector3D((int)(inputdir.x * _PrecisionScale), 0, (int)(inputdir.z * _PrecisionScale));

        if (WishDir.magnitude() > _PrecisionScale)
            WishDir = WishDir.normilized();
        WishDir *= crouching ? (!_Running ? _WalkingForce : _RunningForce) / _CrouchingFactor : !_Running ? _WalkingForce : _RunningForce;

        int multi = 1;
        if (WishDir.normilized().dot(_velocity.flattened.normilized()) / _PrecisionScale < 0.3f * _PrecisionScale)
        {
            multi *= _DirectionMultiplier;
        }

        //acceliration - decceliration
        IntVector3D newVel;
        if (WishDir != IntVector3D.zero)
        {
            if (crouching)
            {
                newVel = _velocity.flattened.MoveTowards(WishDir, _WalkingAccel * _CrouchingFactor * multi, _PrecisionScale);

            }
            else if (_Running)
            {
                newVel = _velocity.flattened.MoveTowards(WishDir, _RunningAccel * multi, _PrecisionScale);

            }
            else
            {
                newVel = _velocity.flattened.MoveTowards(WishDir, _WalkingAccel * multi, _PrecisionScale);
            }
        }
        else //deccel
        {
            if (crouching)
            {
                newVel = _velocity.flattened.MoveTowards(IntVector3D.zero, _WalkingDecel * _CrouchingFactor * multi, _PrecisionScale);

            }
            else if (_Running)
            {
                newVel = _velocity.flattened.MoveTowards(IntVector3D.zero, _RunningDecel * multi, _PrecisionScale);

            }
            else
            {
                newVel = _velocity.flattened.MoveTowards(IntVector3D.zero, _WalkingDecel * multi, _PrecisionScale);
            }
        }

        _velocity = new IntVector3D( newVel.x, _velocity.y, newVel.z);

        if (WishDir.magnitude() == 0 && _velocity.flattened.magnitude() < 2000)
        {
            _velocity = new IntVector3D(0, _velocity.y, 0);
        }

        if (_Grounded) //max speeds
        {
            if (crouching)
            {
                if (_velocity.magnitude() > ((_Running ? _RunningMaxSpeed : _WalkingMaxSpeed) / _CrouchingFactor))
                {
                    _velocity.AddAsFlattened(_velocity.flattened.normilized() * ((_Running ? _RunningMaxSpeed : _WalkingMaxSpeed) / _CrouchingFactor));
                }
            }
            else if (_Running)
            {
                if (_velocity.magnitude() > _RunningMaxSpeed)
                {
                    _velocity.AddAsFlattened(_velocity.flattened.normilized() * _RunningMaxSpeed);
                }
            }
            else
            {
                if (_velocity.magnitude() > _WalkingMaxSpeed)
                {
                    _velocity.AddAsFlattened(_velocity.flattened.normilized() * _WalkingMaxSpeed);
                }
            }
        }

        //Jump
        if ((input.inputBits & (uint)InputType.JumpDown) != 0 && _Grounded)
        {
            _velocity.y = _JumpForce;
            _Jumping = true;
        }
    }

    private void NextPhysicsStep()
    {
        AddForces();
        VerticalPhysics();
        CheckCollisions();
        //Debug.Log(_velocity +" , mag:" +_velocity.magnitude());
    }

    private List<GameObject> _deletableCollisions = new List<GameObject>();
    private void CheckCollisions()
    {
        IntVector3D collVel = _velocity;
        foreach (KeyValuePair<GameObject, ContactPoint[]> pair in _Collisions)
        {
            foreach (ContactPoint point in pair.Value)
            {
                if (point.otherCollider && !point.otherCollider.isTrigger)
                {
                    IntVector3D normal = IntVector3D.Convert(point.normal,_PrecisionScale).flattened;
                    IntVector3D surface = new IntVector3D(normal.z, 0, -normal.x);

                    if (collVel.dot(normal) < 0)
                    {
                        IntVector3D newVel = IntVector3D.Project(collVel.flattened, surface);
                        collVel = new IntVector3D(newVel.x, collVel.y, newVel.z);
                    }
                }
                else
                {
                    _deletableCollisions.Add(pair.Key);
                }
            }
        }

        foreach(GameObject g in _deletableCollisions)
        {
            _Collisions.Remove(g);
        }
        _deletableCollisions.Clear();

        _position += (collVel / (int)(1/Time.fixedDeltaTime));
    }

    private void VerticalPhysics()
    {
        _velocity.y -= _Gravity;

        if (_velocity.y < _MaxDownwardsSpeed)
            _velocity.y = _MaxDownwardsSpeed;
        if (_velocity.y > -_MaxDownwardsSpeed)
            _velocity.y = -_MaxDownwardsSpeed;

        //push character upwards
        Ray[] rays = new Ray[]{
            new Ray(_Feet.position + new Vector3(0,0,0), Vector3.down),
            new Ray(_Feet.position + new Vector3(_FeetDistance,0,0), Vector3.down),
            new Ray(_Feet.position + new Vector3(-_FeetDistance,0,0), Vector3.down),
            new Ray(_Feet.position + new Vector3(0,0,-_FeetDistance), Vector3.down),
            new Ray(_Feet.position + new Vector3(0,0,_FeetDistance), Vector3.down)
        };
        _Grounded = false;
        if ((Physics.Raycast(rays[0], out RaycastHit hit, _HoverHeight) ||
            Physics.Raycast(rays[1], out hit, _HoverHeight) ||
            Physics.Raycast(rays[2], out hit, _HoverHeight) ||
            Physics.Raycast(rays[3], out hit, _HoverHeight) ||
            Physics.Raycast(rays[4], out hit, _HoverHeight))
            && !hit.collider.isTrigger)
        {
            _Grounded = true;
            if (!_Jumping)
            {
                _position.y = (int)((hit.point.y + _Feet.localPosition.y) * _PrecisionScale);
                _velocity.y = 0;
            }
        }
        else if (_Jumping)
            _Jumping = false;

        //Stop going up when colliding with roof
        Ray ray = new Ray(transform.position + new Vector3(0,1,0), Vector3.up);
        if(Physics.Raycast(ray, out RaycastHit hit2,0.2f) && _velocity.y > 0 && !hit2.collider.isTrigger)
        {
            _velocity.y = 0;
        }
    }

    private void ApplyPosition()
    {
        _rb.position = Vector3.Scale(new Vector3(_position.x, _position.y, _position.z), _ScaleVector);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_Collisions.ContainsKey(collision.gameObject) && collision.gameObject.layer != _LayerMask)
            _Collisions.Add(collision.gameObject, collision.contacts);
    }
    private void OnCollisionExit(Collision collision)
    {
        _Collisions.Remove(collision.gameObject);
    }

    public void AddInputs(List<InputBits> inputs)
    {
        foreach (InputBits input in inputs)
            _InputQueue.AddToFront(input);
    }

    void HandleMouse(float x, float y)
    {
        y *= _LookSpeed;
        x *= _LookSpeed;
        _RotationX += -y;
        _RotationX = Mathf.Clamp(_RotationX, -_LookLimit, _LookLimit);

        _CamTran.localRotation = Quaternion.Euler(_RotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, x, 0);
    }

    private void AddForces()
    {
        foreach(KeyValuePair<IntVector3D,ForceMode> pair in _AdditiveForcesBuffer)
        {
            switch (pair.Value)
            {
                case ForceMode.Force:
                    _velocity += pair.Key;
                    break;
                case ForceMode.Impulse:
                    _velocity = pair.Key;
                    break;
                default:
                    Debug.LogError("ForceMode type not Defined");
                    break;
            }
        }
        _AdditiveForcesBuffer.Clear();

        //friction
        IntVector3D friction = -_velocity.flattened.normilized() * _Friction;
        _velocity += friction / _PrecisionScale;
    }

    public void AddForce(IntVector3D force, ForceMode forcemode)
    {
        _AdditiveForcesBuffer.Add(new KeyValuePair<IntVector3D,ForceMode>(force,forcemode));
    }

    public void SetPosition(IntVector3D pos)
    {
        _position = pos;
    }

    public void SetJumping(bool b) => _Jumping = b;

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //bottom
        Gizmos.DrawLine(_Feet.transform.position + new Vector3(0, 0, 0), _Feet.transform.position + new Vector3(0, 0, 0) + (Vector3.down * (_HoverHeight)));
        Gizmos.DrawLine(_Feet.transform.position + new Vector3(_FeetDistance, 0, 0), _Feet.transform.position + new Vector3(_FeetDistance, 0, 0) + (Vector3.down * (_HoverHeight)));
        Gizmos.DrawLine(_Feet.transform.position + new Vector3(-_FeetDistance, 0, 0), _Feet.transform.position + new Vector3(-_FeetDistance, 0, 0) + (Vector3.down * (_HoverHeight)));
        Gizmos.DrawLine(_Feet.transform.position + new Vector3(0, 0, _FeetDistance), _Feet.transform.position + new Vector3(0, 0, _FeetDistance) + (Vector3.down * (_HoverHeight)));
        Gizmos.DrawLine(_Feet.transform.position + new Vector3(0, 0, -_FeetDistance), _Feet.transform.position + new Vector3(0, 0, -_FeetDistance) + (Vector3.down * (_HoverHeight)));

        //head
        Gizmos.DrawLine(transform.position + new Vector3(0, _FeetDistance, 0),transform.position + new Vector3(0,1.5f,0));
        if(_Crouching)
            Gizmos.DrawLine(transform.position + new Vector3(0.2f, 0, 0.2f), transform.position + new Vector3(0.2f, 0, 0.2f) + Vector3.up);
    }

#endif
}

[Flags]
public enum InputType : uint
{
    Ultimate = 1,
    Ability1 = 2,
    Ability2 = 4,
    Ability3 = 8,

    Ability4 = 16,
    Emote = 32,
    Left = 64,
    Right = 128,

    Up = 256,
    Down = 512,
    Running = 1024,
    JumpDown = 2048,

    JumpUp = 4096,
    Crouching = 8192,
    Aiming = 16_384,
    Interact = 32_768,
}

public struct InputBits
{
    public float x;
    public float y;
    public uint inputBits;
}

[Serializable]
public struct IntVector3D
{
    public int x;
    public int y;
    public int z;

    public IntVector3D(int x, int y, int z)
    {
        this.x = x; this.y = y; this.z = z;
    }

    public static IntVector3D zero { get { return new IntVector3D(); } }
    public static IntVector3D up { get { return new IntVector3D(0,1000,0); } }
    public static IntVector3D forward { get { return new IntVector3D(0,0,1000); } }
    public static IntVector3D right { get { return new IntVector3D(1000,0,0); } }
    public int magnitude ()
    {
        return (int)Mathf.Sqrt((x * x) + (y * y) + (z * z));
    }
    public IntVector3D normilized ()
    {
        IntVector3D result = new IntVector3D(x * 1000, y * 1000, z * 1000);
        int mag = magnitude();
        if (mag != 0)
            return new IntVector3D(result.x / mag, result.y / mag, result.z / mag);
        else
            return IntVector3D.zero;
    }

    public static IntVector3D Convert(Vector3 vector, int scale)
    {
        return new IntVector3D((int)(vector.x * scale), (int)(vector.y * scale), (int)(vector.z * scale));
    }

    public IntVector3D MoveTowards(IntVector3D other, int stepsize, int scale)
    {
        return this + (((other - this).normilized() * stepsize)/scale);
    }

    public static IntVector3D operator +(IntVector3D lhs, IntVector3D rhs)
    {
        lhs.x += rhs.x;
        lhs.y += rhs.y;
        lhs.z += rhs.z;
        return lhs;
    }
    public static IntVector3D operator -(IntVector3D lhs, IntVector3D rhs)
    {
        lhs.x -= rhs.x;
        lhs.y -= rhs.y;
        lhs.z -= rhs.z;
        return lhs;
    }
    public static bool operator ==(IntVector3D lhs, IntVector3D rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
    }
    public static bool operator !=(IntVector3D lhs, IntVector3D rhs)
    {
        return lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z;
    }
    public static IntVector3D operator-(IntVector3D rhs)
    {
        return new IntVector3D(-rhs.x,-rhs.y,-rhs.z);
    }
    public static IntVector3D operator *(IntVector3D lhs,int rhs)
    {
        return new IntVector3D(lhs.x * rhs,lhs.y * rhs, lhs.z*rhs);
    }
    public int dot(IntVector3D other)
    {
        return x * other.x + y * other.y + z * other.z;
    }
    public static IntVector3D operator /(IntVector3D lhs,int rhs)
    {
        return new IntVector3D(lhs.x / rhs,lhs.y / rhs, lhs.z/rhs);
    }
    public static IntVector3D Project(IntVector3D from,IntVector3D onto)
    {
        return onto * (from.dot(onto) / onto.dot(onto));
    }
    public IntVector3D flattened { get { return new IntVector3D(x, 0, z); } }
    public IntVector3D AddAsFlattened(IntVector3D other)
    {
        return new IntVector3D(x+other.x,y,z+other.z);
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
    public override bool Equals(object obj)
    {
        return this == (IntVector3D)obj;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
