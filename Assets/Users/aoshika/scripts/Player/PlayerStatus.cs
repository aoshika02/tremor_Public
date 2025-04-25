using UniRx;
using System;

public class PlayerStatus : SingletonMonoBehaviour<PlayerStatus>
{
    public float San { get; private set; } = 100;
    public IObservable<float> SanObservable => _sanSubject;
    private Subject<float> _sanSubject = new Subject<float>();

    public float Stamina { get; private set; } = 100;
    public IObservable<float> StaminaObservable => _staminaSubject;
    private Subject<float> _staminaSubject = new Subject<float>();

    public void AddSAN(float add)
    {
        if (add > 0)
        {
            San = San + add;
            if (San > 100)
            {
                San = 100;
            }
        }
        _sanSubject.OnNext(San);
    }
    public void SubSAN(float sub)
    {
        if (sub > 0)
        {
            San = San - sub;
            if (San < 0)
            {
                San = 0;
            }
        }
        _sanSubject.OnNext(San);
    }
    public void AddStamina(float add)
    {
        Stamina = Stamina + add;
        if (Stamina > 100)
        {
            Stamina = 100;
        }
        _staminaSubject.OnNext(Stamina);
    }
    public void SubStamina(float sub)
    {
        Stamina = Stamina - sub;
        if (Stamina < 0)
        {
            Stamina = 0;
        }
        _staminaSubject.OnNext(Stamina);
    }
}
