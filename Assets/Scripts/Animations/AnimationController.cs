using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Assertions;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private Unit _unit;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _unit = GetComponent<Unit>();

        Assert.IsNotNull( _animator, "Animator not assigned in " + gameObject.name);
        Assert.IsNotNull(_unit, "Unit not Assigned in" + gameObject.name);

        _unit.OnNewCommand += ListenForStateChange;
        PauseManager.Instance.OnPauseStart += OnPauseStart;
        PauseManager.Instance.OnPauseEnd += OnPauseStop;
    }

    private void LateUpdate()
    {
        if (_unit.NavAgent.velocity.magnitude > .2f)
        {
            Vector3 normalizedVelocity = _unit.NavAgent.velocity.normalized;

            float velocityZ = Vector3.Dot(normalizedVelocity, transform.forward);
            float velocityX = Vector3.Dot(normalizedVelocity, transform.right);

            _animator.SetFloat("VelocityZ", velocityZ, 0.1f, Time.deltaTime);
            _animator.SetFloat("VelocityX", velocityX, 0.1f, Time.deltaTime);
        }
        else
        {
            _animator.SetFloat("VelocityZ", 0, 0.1f, Time.deltaTime);
            _animator.SetFloat("VelocityX", 0, 0.1f, Time.deltaTime);
        }

        _animator.SetFloat("VelocityMagnitude", _unit.NavAgent.velocity.magnitude, 0.1f, Time.deltaTime);
    }

    private void OnPauseStart()
    {
        _animator.speed = 0;
    }

    private void OnPauseStop()
    {
        _animator.speed = 1f;
    }

    private void OnDisable()
    {
        _unit.OnNewCommand -= ListenForStateChange;
        PauseManager.Instance.OnPauseStart -= OnPauseStart;
        PauseManager.Instance.OnPauseEnd -= OnPauseStop;
    }

    private void ListenForStateChange(Command newCommand)
    {
        System.Type commandType = newCommand.GetType();
        _animator.SetBool("TakingCover", false);
        _animator.SetBool("Kneeling", false);
        _animator.SetBool("Intimidate", false);
        _animator.SetBool("Shooting", false);

        if (commandType == typeof(InteractCommand)) _animator.SetTrigger("Interacting");
        if (commandType == typeof(TakeCoverCommand)) _animator.SetBool("TakingCover", true);
        if (commandType == typeof(SurrenderCommand)) _animator.SetBool("Kneeling", true);
        if (commandType == typeof(IntimidateCommand)) _animator.SetBool("Intimidate", true);
        if (commandType == typeof(NeutralizeEnemyCommand)) _animator.SetTrigger("Neutralize");
        if (commandType == typeof(NeutralizedCommand)) _animator.SetTrigger("Death");
        if (commandType == typeof(ReloadCommand)) _animator.SetTrigger("Reload");
        if (commandType == typeof(ShootCommand)) _animator.SetBool("Shooting", true);
    }
}
