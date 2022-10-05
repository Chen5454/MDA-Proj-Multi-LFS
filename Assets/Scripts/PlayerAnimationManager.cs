using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{

    public Animator _playerAnimator;
    private PlayerController _player;


    private void Awake()
    {
        _player = GetComponent<PlayerController>();
    }



    public void IdleStateAnimation()
    {
        _playerAnimator.SetFloat("VelocityZ", 0f, 0.1f, Time.deltaTime);
        _playerAnimator.SetFloat("VelocityX", 0f, 0.1f, Time.deltaTime);
    }


    public void MoveStateAnimation()
    {
        if (_player._input.y > 0)
        {
            _playerAnimator.SetFloat("VelocityZ", _player.actualSpeed == _player._walkingSpeed ? 0.5f : 1f, 0.1f,
                Time.deltaTime);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
                _playerAnimator.SetFloat("VelocityZ", -1.0f, 0.1f, Time.deltaTime);
            else
                _playerAnimator.SetFloat("VelocityZ", -0.5f, 0.1f, Time.deltaTime);
        }
    }


    public void RotateAnimation()
    {
        _playerAnimator.SetFloat("VelocityX", _player._input.x, 0.1f, Time.deltaTime);

    }
}
