using UnityEngine;

public class LumberjackCharacter : FirstPersonMovement
{
    [SerializeField] private ThrowWeapon _weaponAxe;
    private int _axesAvailable;

    protected override void Update()
    {
        base.Update();
        InputHolder();

        //Weapon Animation
        if (_weaponAxe != null)
        {
            _weaponAxe.UpdateSway();
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
                _weaponAxe.UpdateBreath(WalkMode.Idle);
            else if (Input.GetKey(KeyCode.LeftShift))
                _weaponAxe.UpdateBreath(WalkMode.Run);
            else
                _weaponAxe.UpdateBreath(WalkMode.Walk);
        }
    }

    void InputHolder()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) Attack();
    }

    void Attack()
    {
        if (_axesAvailable > 0)
        {
            _axesAvailable--;
            _weaponAxe.Attack();
        }
    }
}
