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
        _playerAnimator.SetFloat("Movement Speed", 0f, 0.1f, Time.deltaTime);
        _playerAnimator.SetFloat("Rotatation Speed", 0f, 0.1f, Time.deltaTime);
    }


    public void MoveStateAnimation()
    {
        if (_player._input.y > 0)
        {
            _playerAnimator.SetFloat("Movement Speed", _player.actualSpeed == _player._walkingSpeed ? 0.5f : 1f, 0.1f, Time.deltaTime);
        }
        else
        {
            _playerAnimator.SetFloat("Movement Speed", 0f, 0.1f, Time.deltaTime);
        }
    }


    public void RotateAnimation()
    {
        _playerAnimator.SetFloat("Rotatation Speed", _player._input.x, 0.1f, Time.deltaTime);

    }
}
